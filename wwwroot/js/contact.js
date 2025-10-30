document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('contactForm');
    const result = document.getElementById('result');
    const phoneInput = document.getElementById('phone');
    const emailInput = document.getElementById('email');
    const nameInput = document.getElementById('name');
    const messageInput = document.getElementById('message');
    const consentInput = document.getElementById('consent');

    function validateSwedishPhone(phone) {
        if (!phone) return false;
        const cleaned = phone.replace(/[\s\-]/g, '');
        const patterns = [
            /^(\+46|0046)7[02369]\d{7}$/,
            /^07[02369]\d{7}$/
        ];
        return patterns.some(pattern => pattern.test(cleaned));
    }

    function validateEmail(email) {
        if (!email) return false;
        const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return pattern.test(email);
    }

    function validateField(input, validationFunction) {
        if (!input.value) {
            input.classList.remove('is-valid', 'is-invalid');
            return false;
        }

        if (validationFunction(input.value)) {
            input.classList.remove('is-invalid');
            input.classList.add('is-valid');
            return true;
        } else {
            input.classList.remove('is-valid');
            input.classList.add('is-invalid');
            return false;
        }
    }

    if (phoneInput) {
        phoneInput.addEventListener('input', function () {
            validateField(this, validateSwedishPhone);
        });

        phoneInput.addEventListener('blur', function () {
            validateField(this, validateSwedishPhone);
        });
    }

    if (emailInput) {
        emailInput.addEventListener('input', function () {
            validateField(this, validateEmail);
        });

        emailInput.addEventListener('blur', function () {
            validateField(this, validateEmail);
        });
    }

    if (nameInput) {
        nameInput.addEventListener('input', function () {
            if (this.value.length >= 2) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else if (this.value.length > 0) {
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            }
        });
    }

    if (messageInput) {
        messageInput.addEventListener('input', function () {
            if (this.value.length >= 10) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else if (this.value.length > 0) {
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            }
        });
    }

    if (consentInput) {
        consentInput.addEventListener('change', function () {
            if (this.checked) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            }
        });
    }

    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            let isValid = true;

            if (!nameInput.value || nameInput.value.length < 2) {
                nameInput.classList.add('is-invalid');
                isValid = false;
            }

            if (!validateEmail(emailInput.value)) {
                emailInput.classList.add('is-invalid');
                isValid = false;
            }

            if (!validateSwedishPhone(phoneInput.value)) {
                phoneInput.classList.add('is-invalid');
                isValid = false;
            }

            if (!messageInput.value || messageInput.value.length < 10) {
                messageInput.classList.add('is-invalid');
                isValid = false;
            }

            if (!consentInput.checked) {
                consentInput.classList.add('is-invalid');
                isValid = false;
            }

            if (!isValid) {
                result.innerHTML = '<div class="alert alert-danger">Please fill in all required fields correctly.</div>';
                setTimeout(() => {
                    result.innerHTML = '';
                }, 5000);
                return;
            }

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
                        document.querySelectorAll('.is-valid, .is-invalid').forEach(el => {
                            el.classList.remove('is-valid', 'is-invalid');
                        });
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