const BookingApp = {
    stops: 0,
    currentDirection: 'to',

    init() {
        console.log('BookingApp initializing...');
        this.setupAutocomplete('PickUpAddress');
        this.setupAutocomplete('DropOffAddress');
        this.setupAddStopButton();
        this.updateFlightNumberVisibility();
        this.restoreFormData();
        console.log('BookingApp initialized');
        const form = document.getElementById("bookingForm");
        if (form) {
            form.addEventListener("submit", () => {
                this.saveStopsToLocalStorage();
                window.sessionStorage.setItem("StopsCount", this.stops);
            });
        }

    },

    setupAddStopButton() {
        const btn = document.getElementById('AddStopBtn');
        const firstStop = document.getElementById('optionalField1');
        const secondStop = document.getElementById('optionalField2');
        if (!btn) return;

        const createRemoveButton = (field) => {
            const removeBtn = document.createElement("button");
            removeBtn.type = "button";
            removeBtn.textContent = "Remove stop";
            removeBtn.classList.add("btn-remove-stop");
            removeBtn.addEventListener("click", () => {
                field.style.display = "none";
                field.querySelectorAll("input").forEach(i => i.value = "");
                removeBtn.remove();
                this.stops--;
                btn.disabled = false;
            });
            return removeBtn;
        };

        btn.addEventListener('click', () => {
            this.stops++;
            if (this.stops === 1) {
                firstStop.style.display = 'block';
                setTimeout(() => firstStop.classList.add('show'), 10);
                firstStop.appendChild(createRemoveButton(firstStop));
                this.setupAutocomplete('FirstStop');
            } else if (this.stops === 2) {
                secondStop.style.display = 'block';
                setTimeout(() => secondStop.classList.add('show'), 10);
                secondStop.appendChild(createRemoveButton(secondStop));
                this.setupAutocomplete('SecStop');
                btn.disabled = true;
            }
        });
    },

    updateFlightNumberVisibility() {
        const flightField = document.getElementById('flightNumberField');
        if (flightField)
            flightField.style.display = this.currentDirection === 'from' ? 'block' : 'none';
    },

    setupAutocomplete(fieldId) {
        let sessionToken = crypto.randomUUID();
        const input = document.getElementById(fieldId);
        const suggestions = document.getElementById(`${fieldId}-suggestions`);
        if (!input || !suggestions) return;

        let debounceTimer;
        let lastSelectedValue = '';

        input.addEventListener('input', () => {
            if (input.value !== lastSelectedValue)
                this.clearCoordinates(fieldId);

            clearTimeout(debounceTimer);

            if (input.value.length < 2) {
                suggestions.style.display = 'none';
                return;
            }

            debounceTimer = setTimeout(async () => {
                try {
                    const response = await fetch('https://pegasustransportapi-c4dtcrfwgwcae6fw.swedencentral-01.azurewebsites.net/api/Map/AutoComplete', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            input: input.value,
                            sessionToken: sessionToken
                        })
                    });
                    const result = await response.json();
                    const places = result.data?.suggestions || [];
                    this.showSuggestions(places, suggestions, input, fieldId, sessionToken);
                } catch { }
            }, 800);
        });

        input.addEventListener('coordinatesSet', () => {
            lastSelectedValue = input.value;
        });
    },

    clearCoordinates(fieldId) {
        ['PlaceId', 'Latitude', 'Longitude'].forEach(suffix => {
            const el = document.getElementById(`${fieldId}${suffix}`);
            if (el) el.value = '';
        });
    },

    showSuggestions(places, container, input, fieldId, token) {
        container.innerHTML = '';
        if (!places.length) {
            container.style.display = 'none';
            return;
        }

        places.forEach(place => {
            const item = document.createElement('div');
            item.textContent = place.description;
            item.className = 'list-group-item list-group-item-action';
            item.style.cursor = 'pointer';
            item.addEventListener('click', async () => {
                input.value = place.description;
                container.style.display = 'none';
                const placeIdField = document.getElementById(`${fieldId}PlaceId`);
                if (placeIdField) placeIdField.value = place.placeId;
                await this.getCoordinates(token, place.placeId, fieldId);
                input.dispatchEvent(new CustomEvent('coordinatesSet'));
            });
            container.appendChild(item);
        });

        container.style.display = 'block';
    },

    async getCoordinates(sessionToken, placeId, fieldId) {
        try {
            const params = new URLSearchParams({ placeId, sessionToken });
            const response = await fetch(`https://pegasustransportapi-c4dtcrfwgwcae6fw.swedencentral-01.azurewebsites.net/api/Map/GetLongNLat?${params}`);
            const result = await response.json();
            if (result.data) {
                document.getElementById(`${fieldId}Latitude`).value = result.data.latitude;
                document.getElementById(`${fieldId}Longitude`).value = result.data.longitude;
            }
        } catch { }
    },

    restoreFormData() {
        const form = document.getElementById("bookingForm");
        if (!form) return;
        const inputs = form.querySelectorAll("input, textarea, select");

        inputs.forEach(input => {
            const saved = sessionStorage.getItem(input.id);
            if (saved) input.value = saved;
        });

        inputs.forEach(input => {
            input.addEventListener("input", () => {
                sessionStorage.setItem(input.id, input.value);
            });
        });

        this.restoreStopsFromLocalStorage();

        window.addEventListener("beforeunload", () => {
            inputs.forEach(input => sessionStorage.setItem(input.id, input.value));
        });
    }

};

function setTripDirection(direction) {
    const arlandaData = {
        name: "Arlanda (ARN), Terminal 5, Stockholm-Arlanda, Sverige",
        placeId: "ChIJ_YMtw2OdX0YRM1xOfqKV-FI",
        latitude: 59.6493928,
        longitude: 17.9342942
    };

    const toBtn = document.getElementById("toArlandaBtn");
    const fromBtn = document.getElementById("fromArlandaBtn");

    const pickUp = {
        val: document.getElementById("PickUpAddress"),
        place: document.getElementById("PickUpAddressPlaceId"),
        lat: document.getElementById("PickUpAddressLatitude"),
        lng: document.getElementById("PickUpAddressLongitude")
    };

    const dropOff = {
        val: document.getElementById("DropOffAddress"),
        place: document.getElementById("DropOffAddressPlaceId"),
        lat: document.getElementById("DropOffAddressLatitude"),
        lng: document.getElementById("DropOffAddressLongitude")
    };

    toBtn.classList.remove('active');
    fromBtn.classList.remove('active');

    if (direction === 'from') {
        fromBtn.classList.add('active');
        BookingApp.currentDirection = 'from';

        pickUp.val.value = arlandaData.name;
        pickUp.place.value = arlandaData.placeId;
        pickUp.lat.value = arlandaData.latitude;
        pickUp.lng.value = arlandaData.longitude;

        dropOff.val.value = "";
        dropOff.place.value = "";
        dropOff.lat.value = "";
        dropOff.lng.value = "";
    } else {
        toBtn.classList.add('active');
        BookingApp.currentDirection = 'to';

        pickUp.val.value = "";
        pickUp.place.value = "";
        pickUp.lat.value = "";
        pickUp.lng.value = "";

        dropOff.val.value = arlandaData.name;
        dropOff.place.value = arlandaData.placeId;
        dropOff.lat.value = arlandaData.latitude;
        dropOff.lng.value = arlandaData.longitude;
    }

    BookingApp.updateFlightNumberVisibility();
}

document.addEventListener('DOMContentLoaded', () => BookingApp.init());

window.addEventListener('pageshow', (event) => {
    if (event.persisted) {
        BookingApp.restoreFormData();
    }
});

BookingApp.saveStopsToLocalStorage = function () {
    const stops = [];
    if (document.getElementById("FirstStop")?.value)
        stops.push(document.getElementById("FirstStop").value);
    if (document.getElementById("SecStop")?.value)
        stops.push(document.getElementById("SecStop").value);
    localStorage.setItem("BookingStops", JSON.stringify(stops));
};

BookingApp.restoreStopsFromLocalStorage = function () {
    const saved = JSON.parse(localStorage.getItem("BookingStops") || "[]");
    const savedCount = parseInt(sessionStorage.getItem("StopsCount") || "0");

    if (savedCount > 0 && saved.length > 0) {
        const btn = document.getElementById("AddStopBtn");
        if (!btn) return;

        saved.forEach((stop, i) => {
            btn.click();
            const id = i === 0 ? "FirstStop" : "SecStop";
            const el = document.getElementById(id);
            if (el) el.value = stop;
        });
    }
};


document.addEventListener("DOMContentLoaded", function () {
    const pickupInput = document.querySelector("input[type='datetime-local'][name='PickUpDateTime']");
    if (!pickupInput) return;

    pickupInput.setAttribute("step", "300");

    pickupInput.addEventListener("input", () => {
        const value = pickupInput.value;
        if (!value) return;

        const date = new Date(value);
        const minutes = date.getMinutes();
        const remainder = minutes % 5;

        if (remainder !== 0) {
            const adjustedMinutes = minutes + (5 - remainder);
            date.setMinutes(adjustedMinutes);
            date.setSeconds(0, 0);
            pickupInput.value = toLocalISO(date);
        }
    });

    const now = new Date();
    const minDate = new Date(now.getTime() + (48 * 60 + 5) * 60 * 1000);
    const maxDate = new Date(now.getTime() + 60 * 24 * 60 * 60 * 1000);

    const minutes = minDate.getMinutes();
    const remainder = minutes % 5;
    if (remainder !== 0) {
        minDate.setMinutes(minutes + (remainder >= 2.5 ? 5 - remainder : -remainder));
    }
    minDate.setSeconds(0, 0);

    function toLocalISO(date) {
        const tzOffset = date.getTimezoneOffset() * 60000;
        const local = new Date(date.getTime() - tzOffset);
        return local.toISOString().slice(0, 16);
    }


    pickupInput.min = toLocalISO(minDate);
    pickupInput.max = toLocalISO(maxDate);
    pickupInput.value = toLocalISO(minDate);

    pickupInput.addEventListener("change", () => {
        const selected = new Date(pickupInput.value);
        const minutes = selected.getMinutes();
        const roundedMinutes = Math.round(minutes / 5) * 5;
        selected.setMinutes(roundedMinutes);
        selected.setSeconds(0, 0);

        if (selected < minDate) {
            Swal.fire({
                icon: "warning",
                title: "Too Early to Book",
                html: `
            <div class="swal-booking-warning">
                <p>Bookings must be made at least <strong>48 hours</strong> in advance.</p>
                <p class="swal-support-text">
                    If you need a ride sooner, please call our support team.
                </p>
            </div>`,
                confirmButtonText: "Okay, got it!",
                background: "var(--old-lace)",
                color: "var(--oxford-blue)",
                customClass: {
                    popup: "swal-booking-popup",
                    confirmButton: "swal-confirm-btn"
                },
                backdrop: "rgba(3, 34, 64, 0.6)",
                scrollbarPadding: false
            }).then(() => {
                pickupInput.value = toLocalISO(minDate);
                pickupInput.dispatchEvent(new Event("input"));
            });
            return;
        }

        const step = 5;
        const remainderStep = selected.getMinutes() % step;
        if (remainderStep !== 0) {
            const adjustedMinutes = selected.getMinutes() + (step - remainderStep);
            selected.setMinutes(adjustedMinutes);
            pickupInput.value = toLocalISO(selected);
            pickupInput.dispatchEvent(new Event("input"));
        }
    });
});

