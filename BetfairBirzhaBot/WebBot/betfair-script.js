async function CollectAllUrls(x) {
    let result = [];
    let elements = document.querySelectorAll('a[data-event-type-name="' + x + '"]');

    for (let el of elements)
        result.push("https://www.betfair.com/exchange/plus/" + el.getAttribute('href'));
    //if (el.querySelector('data-bf-livescores-match-scores') != null)

    return JSON.stringify(result);
}

async function IsPageLoaded() {
    return document.querySelectorAll('td.coupon-runners').length > 0;
}

async function Login(username, password) {
    document.getElementById('ssc-liu').value = username;
    fireEvent(document.getElementById('ssc-liu'), 'keyup');

    document.getElementById("ssc-lipw").value = password;
    fireEvent(document.getElementById("ssc-lipw"), 'keyup');

    await sleep(500);
    document.getElementById('ssc-lis').click();
}

async function IsLogined() {
    return document.querySelectorAll('form.ssc-lof')[0] && !document.getElementById('ssc-liu');
}



function fireEvent(element, event) {
    const evt = document.createEvent("HTMLEvents");
    evt.initEvent(event, true, true);
    element.dispatchEvent(evt);
}

async function sleep(msec) { return new Promise(resolve => setTimeout(resolve, msec)); }