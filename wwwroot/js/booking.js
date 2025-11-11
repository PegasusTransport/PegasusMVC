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
                firstStop.appendChild(createRemoveButton(firstStop));
                this.setupAutocomplete('FirstStop');
            } else if (this.stops === 2) {
                secondStop.style.display = 'block';
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
            const saved = localStorage.getItem(input.id);
            if (saved) input.value = saved;
        });

        inputs.forEach(input => {
            input.addEventListener("input", () => {
                localStorage.setItem(input.id, input.value);
            });
        });

        form.addEventListener("submit", () => {
            inputs.forEach(input => localStorage.removeItem(input.id));
        });

        window.addEventListener("beforeunload", () => {
            inputs.forEach(input => localStorage.setItem(input.id, input.value));
        });
    }
};

function setTripDirection(direction) {
    const arlandaData = {
        name: "Arlanda Airport (ARN), Stockholm-Arlanda, Sverige",
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


