document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('contactForm');
    const result = document.getElementById('result');

    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            const formData = new FormData(form);
            const object = Object.fromEntries(formData);
            const json = JSON.stringify(object);

            result.innerHTML = '<div class="alert alert-info">Sending message...</div>';

            fetch('https://api.web3forms.com/submit', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: json
            })
                .then(async (response) => {
                    let json = await response.json();
                    if (response.status == 200) {
                        result.innerHTML = '<div class="alert alert-success">Thank you for your message! We will get back to you shortly.</div>';
                        form.reset();
                    } else {
                        console.log(response);
                        result.innerHTML = '<div class="alert alert-danger">' + json.message + '</div>';
                    }
                })
                .catch(error => {
                    console.log(error);
                    result.innerHTML = '<div class="alert alert-danger">Something went wrong. Please try again later.</div>';
                })
                .then(function () {
                    setTimeout(() => {
                        result.style.display = 'none';
                    }, 5000);
                });
        });
    }
});