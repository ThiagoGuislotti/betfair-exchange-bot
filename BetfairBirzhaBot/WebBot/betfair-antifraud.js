(async function () {
    'use strict';

    async function spoofWebRTC({webRTCIp}) {
        const IP_RE = /([\W])\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}([\W])/gm
        const IP_V6_RE = / ([A-Fa-f0-9]{1,4}::?){1,7}[A-Fa-f0-9]{1,4} /gm

        const fillerString = '%FILLER_FOR_IP%'

        async function renewPublicIp() {
            try {
                const newPublicIp = (
                    await (await fetch('https://api.ipify.org/?format=json')).json()
                ).ip;

                webRTCIp = newPublicIp || webRTCIp
            } catch (e) {
            }
        }

        function brodcastIceCandBySample({EvSample, ipAddresses, targetFunc}) {
            for (const ipAddr of ipAddresses) {
                const newEv = {...EvSample}

                if (newEv.candidate) {
                    newEv.candidate = {...EvSample.candidate}

                    newEv.candidate.address = ipAddr
                    newEv.test = ipAddr
                    newEv.candidate.candidate = newEv.candidate.candidate.replace(
                        fillerString,
                        ipAddr
                    )
                }
                newEv.candidate = new Proxy(
                    newEv.candidate,
                    makeHandler({
                        proxiedObj: newEv.candidate,
                        objName: 'newEv.candidate',
                        doNotModify: true
                    })
                )
                const proxyEv = new Proxy(
                    newEv,
                    makeHandler({
                        proxiedObj: newEv,
                        objName: 'newEv',
                        doNotModify: true
                    })
                )

                targetFunc(proxyEv)
            }
            const nullCandidate = {...EvSample}
            nullCandidate.candidate = null
            targetFunc(nullCandidate)
        }

        function makeHandler({proxiedObj, objName, doNotModify = false}) {
            return {
                apply(target, thisArg,) {
                    return Reflect.apply(target, thisArg, parameters)
                },
                construct(target, params) {

                    const obj = Reflect.construct(target, params)

                    const res = new Proxy(
                        obj,
                        makeHandler({
                            proxiedObj: obj,
                            objName: `spawned${objName}`,
                            doNotModify
                        })
                    )

                    return res
                },
                get(target, prop) {
                    const result = Reflect.get(target, prop)

                    if (!doNotModify) {
                        if (prop === 'bind') {
                            return function () {
                                return new Proxy(target, {
                                    construct() {
                                        return new window.RTCPeerConnection({
                                            iceServers: [{urls: 'stun:stun.l.google.com:19302'}]
                                        })
                                    }
                                })
                            }
                        }

                        debugger;

                        if (
                            prop === 'localDescription' ||
                            prop === 'pendingLocalDescription'
                        ) {
                            if (result) {
                                result.sdp = result.sdp.replace(IP_RE, `$1${webRTCIp}$2`)
                                result.sdp = result.sdp.replace(IP_V6_RE, ` ${webRTCIp} `)
                                result.sdp = result.sdp.replaceAll(
                                    `IN IP4 ${webRTCIp}`,
                                    'IN IP4 127.0.0.1'
                                )
                            }
                        } else if (prop === 'createOffer') {

                            return async function () {
                                await renewPublicIp()
                                return await result.bind(proxiedObj)(...arguments)
                            }
                        }
                    }

                    if (typeof result === 'function') {
                        return result.bind(proxiedObj)
                    } else {
                        return result
                    }
                },
                set(target, prop, value) {

                    if (!doNotModify && prop === 'onicecandidate') {
                        if (value === undefined) {
                            return Reflect.set(target, prop, undefined)
                        }
                        return Reflect.set(target, prop, async function (ev) {
                            if (window.___lastTarget === ev.target) return
                            window.___lastTarget = ev.target

                            const fakeEv = {
                                isTrusted: true,
                                bubbles: false,
                                cancelBubble: false,
                                cancelable: false,
                                candidate: ev.candidate,
                                currentTarget: ev.currentTarget,
                                defaultPrevented: false,
                                eventPhase: 0,
                                path: [],
                                returnValue: true,
                                srcElement: ev.srcElement,
                                target: ev.target,
                                timeStamp: ev.timeStamp,
                                type: ev.type
                            }

                            fakeEv.toString = () => '[object RTCPeerConnectionIceEvent]'
                            fakeEv.toLocalString = undefined

                            if (ev.candidate) {
                                const relatedAddress =
                                    ev.candidate.relatedAddress &&
                                    ev.candidate.relatedAddress
                                        .replaceAll('[', '')
                                        .replaceAll(']', '')

                                const newCandidate = {
                                    candidate: ev.candidate.candidate.replaceAll(
                                        ev.candidate.address &&
                                        ev.candidate.address
                                            .replaceAll('[', '')
                                            .replaceAll(']', ''),
                                        fillerString
                                    ),
                                    address: fillerString,
                                    component: ev.candidate.component,
                                    foundation: ev.candidate.foundation,
                                    port: ev.candidate.port,
                                    priority: ev.candidate.priority,
                                    protocol: ev.candidate.protocol,
                                    relatedAddress,
                                    relatedPort: ev.candidate.relatedPort,
                                    sdpMLineIndex: ev.candidate.sdpMLineIndex,
                                    sdpMid: ev.candidate.sdpMid,
                                    tcpType: ev.candidate.tcpType,
                                    type: ev.candidate.type,
                                    usernameFragment: ev.candidate.usernameFragment
                                }
                                newCandidate.toString = () => '[object RTCIceCandidate]'
                                const toJSON = function () {
                                    return {
                                        candidate: this.candidate,
                                        sdpMLineIndex: this.sdpMLineIndex,
                                        sdpMid: this.sdpMid,
                                        usernameFragment: this.usernameFragment
                                    }
                                }
                                toJSON.toString = () => 'toJSON() { [native code] }'
                                newCandidate.toJSON = toJSON
                                newCandidate.toLocalString = undefined
                                fakeEv.candidate = newCandidate

                            }

                            brodcastIceCandBySample({
                                EvSample: fakeEv,
                                ipAddresses: [webRTCIp],
                                targetFunc: value
                            })
                        })
                    } else {
                        return Reflect.set(target, prop, value)
                    }
                }
            }
        }

        const rtcObjs = ['RTCPeerConnection', 'webkitRTCPeerConnection']

        for (const objName of rtcObjs) {
            window[objName] = new Proxy(
                window[objName],
                makeHandler({
                    proxiedObj: window[objName],
                    objName
                })
            )
        }
    }

    // Подмена WebGL
    const injectWebGl = (fakeProfile) => {

        if (!fakeProfile.WebGlFake.Status ||
            fakeProfile.WebGlFake.UnmaskedRenderer === 'System VideoCard' ||
            fakeProfile.WebGlFake.UnmaskedRenderer === '') {
            return;
        }
        const webglUtil = {
            random: {
                value() {
                    return Math.random();
                },
                item(e) {
                    const rand = e.length * webglUtil.random.value();
                    return e[Math.floor(rand)];
                },
                array(e) {
                    const rand = webglUtil.random.item(e);
                    return new Int32Array([rand, rand]);
                },
                items(e, n) {
                    let {length} = e;
                    const result = new Array(n);
                    const taken = new Array(length);
                    if (n > length) {
                        // eslint-disable-next-line no-param-reassign
                        n = length;
                    }
                    //
                    // eslint-disable-next-line no-plusplus, no-param-reassign
                    while (n--) {
                        const i = Math.floor(webglUtil.random.value() * length);
                        result[n] = e[i in taken ? taken[i] : i];
                        // eslint-disable-next-line no-plusplus
                        taken[i] = --length in taken ? taken[length] : length;
                    }
                    return result;
                },
            },
            spoof: {
                webgl: {
                    extensions(target) {
                        const extensions = [
                            'ANGLE_instanced_arrays',
                            'EXT_blend_minmax',
                            'EXT_color_buffer_half_float',
                            'EXT_disjoint_timer_query',
                            'EXT_float_blend',
                            'EXT_frag_depth',
                            'EXT_shader_texture_lod',
                            'EXT_texture_compression_bptc',
                            'EXT_texture_compression_rgtc',
                            'EXT_texture_filter_anisotropic',
                            'WEBKIT_EXT_texture_filter_anisotropic',
                            'EXT_sRGB',
                            'KHR_parallel_shader_compile',
                            'OES_element_index_uint',
                            'OES_fbo_render_mipmap',
                            'OES_standard_derivatives',
                            'OES_texture_float',
                            'OES_texture_float_linear',
                            'OES_texture_half_float',
                            'OES_texture_half_float_linear',
                            'OES_vertex_array_object',
                            'WEBGL_color_buffer_float',
                            'WEBGL_compressed_texture_s3tc',
                            'WEBKIT_WEBGL_compressed_texture_s3tc',
                            'WEBGL_compressed_texture_s3tc_srgb',
                            'WEBGL_debug_renderer_info',
                            'WEBGL_debug_shaders',
                            'WEBGL_depth_texture',
                            'WEBKIT_WEBGL_depth_texture',
                            'WEBGL_draw_buffers',
                            'WEBGL_lose_context',
                            'WEBKIT_WEBGL_lose_context',
                            'WEBGL_multi_draw',
                        ];
                        Object.defineProperty(target.prototype, 'getSupportedExtensions', {
                            value() {
                                return extensions;
                            },
                        });
                    },
                    buffer(target) {
                        const {bufferData} = target.prototype;
                        Object.defineProperty(target.prototype, 'bufferData', {
                            value(_target, srcData) {
                                let fakeNoise = fakeProfile.WebGlFake["0"];
                                debugger;
                                while (fakeNoise > 1) {
                                    fakeNoise *= 0.1;
                                }
                                fakeNoise *= 0.1;
                                srcData.forEach((val, index) => {
                                    if (val === 0) {
                                        return;
                                    }
                                    const noise = fakeNoise * val;
                                    // eslint-disable-next-line no-param-reassign
                                    srcData[index] += noise;
                                });
                                // eslint-disable-next-line prefer-rest-params
                                return bufferData.apply(this, arguments);
                            },
                        });
                    },
                    parameter(target) {
                        const {getParameter} = target.prototype;
                        Object.defineProperty(target.prototype, 'getParameter', {
                            configurable: false,
                            enumerable: true,
                            writable: true,
                            value(pname) {
                                // const float32array = new Float32Array([1, 8192]);
                                if (pname === 37446) {
                                    return fakeProfile.WebGlFake.UnmaskedRenderer;
                                }
                                if (pname === 37445) {
                                    return 'Google inc.';
                                }
                                if (fakeProfile.WebGlValues[pname] && pname !== 0) {
                                    return fakeProfile.WebGlValues[pname];
                                }
                                return getParameter.apply(this, [pname]);
                            },
                        });
                    },
                },
            },
        };
        webglUtil.spoof.webgl.extensions(WebGLRenderingContext);
        webglUtil.spoof.webgl.extensions(WebGL2RenderingContext);
        webglUtil.spoof.webgl.buffer(WebGLRenderingContext);
        webglUtil.spoof.webgl.buffer(WebGL2RenderingContext);
        webglUtil.spoof.webgl.parameter(WebGLRenderingContext);
        webglUtil.spoof.webgl.parameter(WebGL2RenderingContext);
    };

    // Подмена CPU и ОП
    function patchNavigatorCpu($window, fakeProfile) {

        overridePropInGet($window.navigator, "deviceMemory", 8);
        overridePropInGet($window.navigator, "hardwareConcurrency", 12);

        const getHighEntropyValues = Object.getPrototypeOf(navigator.userAgentData).getHighEntropyValues;

        Object.defineProperty(Object.getPrototypeOf(navigator.userAgentData), 'getHighEntropyValues', {
            async value(...args) {
                const result = await getHighEntropyValues.apply(this, args);
                result['platformVersion'] = '';
                result['architecture'] = '';
                result['bitness'] = '';
                return result;
            },
        });
    }


    // Проверка батареи
    function patchNavigatorBattery($window) {

        overridePropInValue($window.BatteryManager.prototype, 'level', {
            value: 1, configurable: false, enumerable: true, writable: true
        });

        overridePropInValue($window.BatteryManager.prototype, 'dischargingTime', {
            value: Infinity, configurable: false, enumerable: true, writable: true
        });

        overridePropInValue($window.BatteryManager.prototype, 'charging', {
            value: true, configurable: false, enumerable: true, writable: true
        });

    }

    // Проверка блютуз
    function patchNavigatorBluetooth($window) {
        const blue = {};
        blue.getAvailability = function () {
            return new Promise(function (resolve, reject) {
                resolve(true);
            });
        };
        overridePropInGet($window.navigator.prototype, 'bluetooth', blue);
    }

    function patchNavigatorWebDriver($window) {
        overridePropInGet($window.navigator, 'webdriver', false);
        // Object.defineProperty($window.navigator, 'webdriver', {
        //     get: () => false
        // });
    }

    // NavigatorConnection
    function patchNavigatorConnection($window) {
        function NetworkInformation() {

        }

        overridePropInGet(NetworkInformation.prototype, "downlink", 1.45);
        overridePropInGet(NetworkInformation.prototype, "effectiveType", "4g");

        overridePropInGet(NetworkInformation.prototype, "rtt", 250);
        overridePropInGet(NetworkInformation.prototype, "saveData", false);

        let networkInfo = new NetworkInformation();

        overridePropInGet($window.Navigator.prototype, 'connection', networkInfo,
            {
                __proto__: NetworkInformation.prototype
            });
    }

    // Подмена Аудио отпечатка
    function patchAudio($window, fakeProfile) {

        const canPlayType = HTMLAudioElement.prototype.canPlayType;
        Object.defineProperty(HTMLAudioElement.prototype, 'canPlayType', {
            'value': function () {
                if (arguments[0].includes("aac")) {
                    return "probably";
                }
                if (arguments[0].includes("m4a")) {
                    return "maybe";
                }
                if (arguments[0].includes("mp4")) {
                    return "probably";
                }
                return canPlayType.apply(this, arguments);
            }, configurable: false, enumerable: true, writable: true
        });
        const canPlayTypeV = HTMLVideoElement.prototype.canPlayType;
        Object.defineProperty(HTMLVideoElement.prototype, 'canPlayType', {
            'value': function () {
                if (arguments[0].includes("mp4")) {
                    return "probably";
                }
                return canPlayTypeV.apply(this, arguments);
            }, configurable: false, enumerable: true, writable: true
        });
        const supported = MediaSource.isTypeSupported;
        Object.defineProperty(MediaSource, 'isTypeSupported', {
            'value': function () {
                let argument = arguments[0];
                if (!argument) {
                    return supported.apply(this, arguments);
                }
                if (argument === "video/mp4; codecs=\"avc1.42E01E, mp4a.40.2\"") {
                    return true;
                }
                if (argument === "video/webm; codecs=\"vp8, vorbis\"") {
                    return true;
                }

                return supported.apply(this, arguments);
            }, configurable: false, enumerable: true, writable: true
        });

        overridePropInValue($window.AudioContext.prototype, 'baseLatency', {
            value: fakeProfile.BaseLatency, configurable: false, enumerable: true, writable: true
        });
        var BUFFER = null;

        function patchChannelDelta(e) {
            const getChannelData = e.prototype.getChannelData;
            Object.defineProperty(e.prototype, 'getChannelData', {
                'value': function () {
                    const results1 = getChannelData.apply(this, arguments);
                    if (BUFFER !== results1) {
                        BUFFER = results1;
                        for (var i = 0; i < results1.length; i += 100) {
                            let index = Math.floor(fakeProfile.ChannelDataIndexDelta * i);
                            results1[index] = results1[index] + fakeProfile.ChannelDataDelta * 0.0000001;
                            debugger;
                        }
                    }
                    return results1;
                }, onfigurable: false, enumerable: true, writable: true
            });

            Object.defineProperty(e.prototype.getChannelData, 'toString', {
                "value": function () {
                    return 'getChannelData() { [native code] }';
                }
            });

            Object.defineProperty(e.prototype.getChannelData.toString, 'toString', {
                "value": function () {
                    return 'toString() { [native code] }';
                }
            });
        }

        function createAnalyzer(e) {
            const createAnalyser = e.prototype.__proto__.createAnalyser;
            Object.defineProperty(e.prototype.__proto__, 'createAnalyser', {
                'value': function () {
                    const results2 = createAnalyser.apply(this, arguments);
                    const getFloatFrequencyData = results2.__proto__.getFloatFrequencyData;
                    Object.defineProperty(results2.__proto__, 'getFloatFrequencyData', {
                        'value': function () {
                            const results3 = getFloatFrequencyData.apply(this, arguments);
                            for (var i = 0; i < arguments[0].length; i += 100) {
                                let index = Math.floor(fakeProfile.FloatFrequencyDataIndexDelta * i);
                                arguments[0][index] = arguments[0][index] + fakeProfile.FloatFrequencyDataDelta * 0.1;
                            }
                            return results3;
                        }, configurable: false, enumerable: true, writable: true
                    });
                    Object.defineProperty(results2.__proto__.getFloatFrequencyData, 'toString', {
                        "value": function () {
                            return 'getFloatFrequencyData() { [native code] }';
                        }
                    });

                    Object.defineProperty(results2.__proto__.getFloatFrequencyData.toString, 'toString', {
                        "value": function () {
                            return 'toString() { [native code] }';
                        }
                    });

                    return results2;
                }, configurable: false, enumerable: true, writable: true
            });
        }

        patchChannelDelta(AudioBuffer)
        createAnalyzer(AudioContext)
        createAnalyzer(OfflineAudioContext)
    }

    // Property
    function overridePropInValue(obj, name, attrs, otherattrs) {
        //Object.defineProperty(obj, name, attrs);

        if (typeof attrs.value === "function") {
            if (otherattrs && otherattrs.argscount) {
                overridePropInGet(obj[name], "length", otherattrs.argscount);
            }

            let tostr = attrs.value.toString = function () {
                return "function " + name + '() { [native code] }';
            }
            Object.defineProperty(tostr, "name", {value: "toString"});

            for (var i = 0; i < 100; i++) {
                tostr.toString = function () {
                    return 'function toString() { [native code] }';
                }
                Object.defineProperty(tostr, "name", {value: "toString"});

                tostr = tostr.toString;
            }
        }
    }

    function overridePropInGet(obj, name, f, attrs) {
        let getter = function () {
            return f;
        };
        Object.defineProperty(getter, "name", {value: "get " + name});
        let tostr2 = getter.toString = function () {
            return "function get " + name + '() { [native code] }';
        };
        let maxcnt = 101;
        for (var i = 0; i < maxcnt; i++) {
            tostr2.toString = function () {
                return 'function toString() { [native code] }';
            }
            Object.defineProperty(tostr2, "name", {value: "toString"});

            tostr2 = tostr2.toString;
        }


        let attributes = {get: getter, enumerable: true, configurable: true,};
        if (attrs) {
            attributes.__proto__ = attrs.__proto__;
        }
        Object.defineProperty(obj, name,attributes);
        if (!!obj[name] && typeof obj[name] === 'function') {

            let tostr = obj[name].toString = function () {
                return "function " + name + '() { [native code] }';
            }
            Object.defineProperty(tostr, "name", {value: "toString"});

            for (var i = 0; i < maxcnt; i++) {
                tostr.toString = function () {
                    return 'function toString() { [native code] }';
                }
                Object.defineProperty(tostr, "name", {value: "toString"});

                tostr = tostr.toString;
            }
        }
        let gettostr = getter.toString = function () {
            return "function get " + name + "() { [native code] }";
        };
        Object.defineProperty(gettostr, "name", {value: "toString"});

        for (var i = 0; i < maxcnt; i++) {

            Object.defineProperty(gettostr, "name", {value: "toString"});

            gettostr.toString = function () {
                return 'function toString() { [native code] }';
            }
            gettostr = gettostr.toString;
        }
    }


    // Патч окна
    function patchNotifications($window) {
        if (!!$window['Notification']) {
            overridePropInGet($window.Notification, "permission", "default");
            return;
        }
        $window['Notification'] || ($window['Notification'] = {
            'permission': 'default', 'close': function () {
            }, 'requestPermission': function () {
                return new Promise(function (_0x544acb, _0x7d8424) {
                    _0x544acb("default");
                });
            }
        });
        overridePropInGet($window.Notification, "permission", "default");
    }

    if (!window.injected) {
        window.injected = true;

        // var uniqProfile;
        //
        // let getdesc = Object.getOwnPropertyDescriptor;
        // overridePropInValue(Object, "getOwnPropertyDescriptor", {
        //     configurable: true,
        //     enumerable: false,
        //     value: getdesc
        // });
        //patchNotifications(window);
        //patchNavigatorCpu(window, uniqProfile);
        //patchNavigatorBattery(window);
        patchNavigatorWebDriver(window);
        //patchNavigatorBluetooth(window);
        //patchNavigatorConnection(window);
        //patchAudio(window, uniqProfile);
        //injectWebGl(uniqProfile);
        //await spoofWebRTC(uniqProfile.ProxyInfo.OuterIp);
    }
}());
