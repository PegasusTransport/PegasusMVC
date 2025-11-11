const BookingApp = {
    stops: 0,
    currentDirection: 'to',

    init() {
        console.log('BookingApp initializing...');
        this.setupAutocomplete('PickUpAddress');
        this.setupAutocomplete('DropOffAddress');
        this.setupAddStopButton();
        this.updateFlightNumberVisibility();
        console.log('BookingApp initialized');
    },

    setupAddStopButton() {
        const btn = document.getElementById('AddStopBtn');
        if (!btn) {
            console.log('Add stop button not found');
            return;
        }

        btn.addEventListener('click', () => {
            this.stops++;
            console.log('Stops:', this.stops);

            if (this.stops === 1) {
                document.getElementById('optionalField1').style.display = 'block';
                this.setupAutocomplete('FirstStop');
            } else if (this.stops === 2) {
                document.getElementById('optionalField2').style.display = 'block';
                this.setupAutocomplete('SecStop');
                btn.disabled = true;
            }
        });
    },

    updateFlightNumberVisibility() {
        const flightField = document.getElementById('flightNumberField');
        if (flightField) {
            flightField.style.display = this.currentDirection === 'from' ? 'block' : 'none';
        }
    },

    setupAutocomplete(fieldId) {
        let sessionToken = crypto.randomUUID();
        const input = document.getElementById(fieldId);
        const suggestions = document.getElementById(`${fieldId}-suggestions`);

        if (!input || !suggestions) {
            return;
        }

        let debounceTimer;
        let lastSelectedValue = '';

        input.addEventListener('input', () => {

            if (input.value !== lastSelectedValue) {
                this.clearCoordinates(fieldId);
            }

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

                } catch (error) {
                }
            }, 800);
        });


        input.addEventListener('coordinatesSet', () => {
            lastSelectedValue = input.value;
        });
    },

    clearCoordinates(fieldId) {
        const placeIdField = document.getElementById(`${fieldId}PlaceId`);
        const latField = document.getElementById(`${fieldId}Latitude`);
        const lonField = document.getElementById(`${fieldId}Longitude`);

        if (placeIdField) placeIdField.value = '';
        if (latField) latField.value = '';
        if (lonField) lonField.value = '';
    },

    showSuggestions(places, container, input, fieldId, token) {
        container.innerHTML = '';

        if (places.length === 0) {
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

                if (placeIdField) {
                    placeIdField.value = place.placeId;
                }

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
                const latField = document.getElementById(`${fieldId}Latitude`);
                const lonField = document.getElementById(`${fieldId}Longitude`);

                if (latField) latField.value = result.data.latitude;
                if (lonField) lonField.value = result.data.longitude;
            }
        } catch (error) {
        }
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

    const pickUpValue = document.getElementById("PickUpAddress");
    const pickUpPlaceId = document.getElementById("PickUpAddressPlaceId");
    const pickUpLat = document.getElementById("PickUpAddressLatitude");
    const pickUpLng = document.getElementById("PickUpAddressLongitude");

    const dropOffValue = document.getElementById("DropOffAddress");
    const dropOffPlaceId = document.getElementById("DropOffAddressPlaceId");
    const dropOffLat = document.getElementById("DropOffAddressLatitude");
    const dropOffLng = document.getElementById("DropOffAddressLongitude");

    toBtn.classList.remove('active');
    fromBtn.classList.remove('active');

    if (direction === 'from') {
        fromBtn.classList.add('active');
        BookingApp.currentDirection = 'from';

        pickUpValue.value = arlandaData.name;
        pickUpPlaceId.value = arlandaData.placeId;
        pickUpLat.value = arlandaData.latitude;
        pickUpLng.value = arlandaData.longitude;

        dropOffValue.value = "";
        dropOffPlaceId.value = "";
        dropOffLat.value = "";
        dropOffLng.value = "";
    } else {
        toBtn.classList.add('active');
        BookingApp.currentDirection = 'to';

        pickUpValue.value = "";
        pickUpPlaceId.value = "";
        pickUpLat.value = "";
        pickUpLng.value = "";

        dropOffValue.value = arlandaData.name;
        dropOffPlaceId.value = arlandaData.placeId;
        dropOffLat.value = arlandaData.latitude;
        dropOffLng.value = arlandaData.longitude;
    }

    BookingApp.updateFlightNumberVisibility();
}

document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM loaded, initializing BookingApp');
    BookingApp.init();
});