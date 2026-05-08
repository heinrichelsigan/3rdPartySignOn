export function acceptPolicy(cookieString) {
    document.cookie = cookieString;
}

// Function to display current cookies
export function showCookies() {
    
    alert("Current cookies: " + document.cookie + "");
}

// Function to clear all cookies
export function deleteCookies() {
    var cookies = document.cookie.split(';');

    // Set expiry date to January 1, 1970 for all cookies
    for (var i = 0; i < cookies.length; i++) {
        document.cookie = cookies[i] + "=; expires=" + new Date(0).toUTCString();
    }
    showCookies(); // Refresh display
}


console.log("After:", document.cookie);

export function clearAllCookies() {
    var cookies = document.cookie.split(';');

    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i];
        var eqPos = cookie.indexOf('=');
        var name = eqPos > -1 ? cookie.substr(0, eqPos).trim() : cookie.trim();

        // Clear cookie for current path
        document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/';

        // Clear cookie for domain
        document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/;domain=' + window.location.hostname;

        // Clear cookie for parent domain
        document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/;domain=.' + window.location.hostname;
    }
}