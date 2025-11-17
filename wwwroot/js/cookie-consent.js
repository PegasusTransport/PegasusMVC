function loadGoogleAnalytics() {
    const script = document.createElement('script');
    script.async = true;
    script.src = 'https://www.googletagmanager.com/gtag/js?id=G-M01C5FP9YV';
    document.head.appendChild(script);

    script.onload = function () {
        window.dataLayer = window.dataLayer || [];
        function gtag() { dataLayer.push(arguments); }
        gtag('js', new Date());
        gtag('config', 'G-M01C5FP9YV', { 'anonymize_ip': true });
    };
}

function acceptCookies() {
    localStorage.setItem('cookieConsent', 'accepted');
    document.getElementById('cookie-banner').classList.remove('show');
    loadGoogleAnalytics();
}

function declineCookies() {
    localStorage.setItem('cookieConsent', 'declined');
    document.getElementById('cookie-banner').classList.remove('show');
}

document.addEventListener('DOMContentLoaded', function () {
    const renewBtn = document.getElementById('renewCookieBtn');
    if (renewBtn) {
        deleteGoogleAnalyticsCookies();
        renewBtn.addEventListener('click', function (e) {
            localStorage.removeItem('cookieConsent');
            document.getElementById('cookie-banner').classList.add('show');
        });
    }
});

function deleteGoogleAnalyticsCookies() {
    const gaCookies = ['_ga', '_gid', '_gat', '_gat_gtag_G_M01C5FP9YV', '_ga_M01C5FP9YV'];

    gaCookies.forEach(function (cookieName) {
        document.cookie = cookieName + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';

        const domain = window.location.hostname;
        document.cookie = cookieName + '=; Path=/; Domain=' + domain + '; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';

        document.cookie = cookieName + '=; Path=/; Domain=.' + domain + '; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    });

    console.log('Google Analytics cookies deleted');
}
window.addEventListener('load', function () {
    const consent = localStorage.getItem('cookieConsent');

    if (consent === 'accepted') {
        loadGoogleAnalytics();
    } else if (!consent) {
        document.getElementById('cookie-banner').classList.add('show');
    }
});