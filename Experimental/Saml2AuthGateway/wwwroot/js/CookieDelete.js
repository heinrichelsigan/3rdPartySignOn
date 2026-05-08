export function deleteAllCookies(logoutUrl, userName) {

    var urlLogoutString = document.getElementById("metaLogoutDirectUrl").content;
    if (urlLogoutString == null || urlLogoutString.trim() == "")
        urlLogoutString = logoutUrl;
        
    var requestCurrentPathString = document.getElementById("metaRequestCurrentPath").content;
    var upath = window.location.pathname;
    if (upath != null && upath.trim() != "" && upath.includes("/Logout")) {
        requestCurrentPathString = upath.replace("/Logout", "/");
    }
        
    var userNameString = document.getElementById("metaUserName").content;
    if (userNameString == null || userNameString.trim() == "")
        userNameString = userName;

    const uQueryString = window.location.search; // ?user=UserName
    const uParams = new URLSearchParams(uQueryString);
    const uName = uParams.get('user');

    if (uName != null && uName.trim() != == "" &&
        (userNameString == null || userNameString.trim() == "")) {
            userNameString = uName;
    }

    var docCookie = document.cookie;
    var cookies = document.cookie.split(';');

    var cookieName = ".AspNetCore.Cookies";
    var cookieContent = "";
    var documentCookieValue = `${cookieName}={cookieContent}; path=` + requestCurrentPathString + `; domain=` + window.location.hostname + `; expires=${new Date(0).toUTCString()}`;

    document.cookie = documentCookieValue;
    // Set expiry date to January 1, 1970 for all cookies
    for (var i = 0; i < cookies.length; i++) {

        var cookieNameValue = cookies[i];
        if (cookieNameValue != null && cookieNameValue.trim() != "") {

            cookieName = cookieNameValue.split('=')[0].trim(); // Get the cookie name

            if (cookieName != ".AspNetCore.Cookies") {
                documentCookieValue = documentCookieValue + ";" + cookieName + "=; path=/; domain=" + window.location.hostname + ";expires=" + new Date(0).toUTCString(); // Clear cookie for current path
                document.cookie = documentCookieValue;
            }
        }
    }

    var logoutButton = document.getElementById("signOutButton");
    var loginButton = document.getElementById("signInButton");

    if (logoutButton != null) {
        logoutButton.click();
        return;
    }
    if (loginButton != null) {
        loginButton.click();
        return;
    }

    const urlWindowLocation = new URL(urlLogoutString);
    var redirectUrlString = urlWindowLocation.toString() + "?logout=" + userNameString;
    redirectUrl = new URL(redirectUrlString);
    setTimeout(function () { redirectToLogout(redirectUrlString) }, 512);
    // window.location.href = redirectUrlString;
}


export function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

export function redirectToLogout(logoutUrl) {
    window.location.href = logoutUrl;
}
