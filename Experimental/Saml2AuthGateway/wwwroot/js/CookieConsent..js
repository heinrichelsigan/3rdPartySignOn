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

export function playSound(soundName) {
	var dursec = 2500;
	document.title = "Fruit Slot Machine";
	let sound = new Audio(soundName);
	sound.autoplay = true;
	sound.loop = false;

	setTimeout(function () {
		try {
			sound.play();
		} catch (soundPlayEx) {
			console.log("playSound(soundName = " + soundName + ") throwed exception: " + soundPlayEx);
		}
	}, 100);

	setTimeout(function () {
		sound.loop = false;
		sound.pause();
		sound.autoplay = false;
		sound.currentTime = 0;
		try {
			sound.src = "";
			sound = null;
		} catch (exSnd) {
		}
		soundDuration = 2500;
	}, dursec);
}

// playSoundUrl
export function playSoundUrl(soundUrl, etext) {
	if (soundUrl != null && soundUrl.length > 1) {
		setTimeout(function () {
			playSound(soundUrl)
		}, 24000);
	}
}