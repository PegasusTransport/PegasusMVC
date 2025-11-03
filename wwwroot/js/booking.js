document.addEventListener('DOMContentLoaded', function () {
    initAutocomplete('PickUpAddress');
    initAutocomplete('DropOffAddress');

    let stops = 0;
    document.getElementById('AddStopBtn').addEventListener('click', function () {
        stops++;
        if (stops === 1) {
            document.getElementById('optionalField1').style.display = 'block';
            initAutocomplete('FirstStop');
        } else if (stops === 2) {
            document.getElementById('optionalField2').style.display = 'block';
            initAutocomplete('SecStop');
        } else if (stops === 3) {
            document.getElementById('optionalField3').style.display = 'block';
            initAutocomplete('ThirdStop');
            this.disabled = true;
        }
    });
});

async function initAutocomplete(id) {
    let token = generateToken();
    const input = document.getElementById(id);
    const suggestions = document.getElementById(id + '-suggestions');
    let timeout;

    input.addEventListener('input', function () {
        clearTimeout(timeout);
        if (this.value.length < 2) {
            suggestions.style.display = 'none';
            return;
        }

        timeout = setTimeout(async () => {
            try {
                const response = await fetch('https://localhost:7161/api/Map/AutoComplete', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ input: input.value, sessionToken: token })
                });

                const data = await response.json();
                const suggestionList = data.data?.suggestions || [];

                suggestions.innerHTML = '';

                if (suggestionList.length === 0) {
                    suggestions.style.display = 'none';
                    return;
                }

                suggestionList.forEach(s => {
                    const div = document.createElement('div');
                    div.textContent = s.description;
                    div.onclick = async () => {
                        input.value = s.description;
                        input.dataset.placeId = s.placeId;
                        suggestions.style.display = 'none';
                        token = generateToken();

                        const placeIdField = document.getElementById(id + "PlaceId");
                        if (placeIdField) {
                            placeIdField.value = s.placeId;  
                        }

                        await sendPlaceDetail(token, s.placeId, placeIdField);
                        console.log(input.value)
                        console.log(s.placeId)
                    };
                    div.style.padding = '10px';
                    div.style.cursor = 'pointer';
                    div.style.borderBottom = '1px solid #eee';
                    div.onmouseover = () => div.style.background = '#f0f0f0';
                    div.onmouseout = () => div.style.background = 'white';
                    
                    suggestions.appendChild(div);
                });

                suggestions.style.display = 'block';
            } catch (error) {
                console.error('Error:', error);
            }
        }, 300);
    });
}

async function sendPlaceDetail(sessionToken, placeId, placeIdField) {
    const params = new URLSearchParams({
        placeId: placeId,
        sessionToken: sessionToken
    });

    const baseUrl = "https://localhost:7161/api/Map/GetLongNLat";
    const url = `${baseUrl}?${params.toString()}`;

    const response = await fetch(url, {
     
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
    });
    const data = await response.json();
}

function generateToken() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
        const r = Math.random() * 16 | 0;
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}

//https://localhost:7161/api/Map/AutoComplete