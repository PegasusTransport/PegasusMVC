const BookingApp = {
    stops: 0,

    init() {
        console.log('BookingApp initializing...');
        this.setupAutocomplete('PickUpAddress');
        this.setupAutocomplete('DropOffAddress');
        this.setupAddStopButton();
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

    setupAutocomplete(fieldId) {
        let sessionToken = crypto.randomUUID();
        const input = document.getElementById(fieldId);
        const suggestions = document.getElementById(`${fieldId}-suggestions`);

        if (!input || !suggestions) {
            console.log(`Autocomplete elements not found for ${fieldId}`);
            return;
        }

        console.log(`Autocomplete setup for ${fieldId}`);
        let debounceTimer;

        input.addEventListener('input', () => {
            clearTimeout(debounceTimer);

            if (input.value.length < 2) {
                suggestions.style.display = 'none';
                return;
            }

            debounceTimer = setTimeout(async () => {
                try {
                    const response = await fetch('https://localhost:7161/api/Map/AutoComplete', {
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
                    console.error('Autocomplete error:', error);
                }
            }, 800);
        });
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
            });

            container.appendChild(item);
        });

        container.style.display = 'block';
    },

    async getCoordinates(sessionToken, placeId, fieldId) {
        try {
            const params = new URLSearchParams({ placeId, sessionToken });
            const response = await fetch(`https://localhost:7161/api/Map/GetLongNLat?${params}`);
            const result = await response.json();

            if (result.data) {
                const latField = document.getElementById(`${fieldId}Latitude`);
                const lonField = document.getElementById(`${fieldId}Longitude`);

                if (latField) latField.value = result.data.latitude;
                if (lonField) lonField.value = result.data.longitude;

                console.log(`Coordinates set for ${fieldId}:`, result.data);
            }
        } catch (error) {
            console.error('Error getting coordinates:', error);
        }
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM loaded, initializing BookingApp');
    BookingApp.init();
});