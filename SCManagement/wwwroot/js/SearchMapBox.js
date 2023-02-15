
mapboxgl.accessToken = 'pk.eyJ1IjoiZXhhbXBsZXMiLCJhIjoiY2p0MG01MXRqMW45cjQzb2R6b2ptc3J4MSJ9.zA2W0IkI0c6KaAhJfk9bWg';
const map = new mapboxgl.Map({
    container: 'map',
    style: 'mapbox://styles/mapbox/streets-v12',
    center: [-8.896442, 38.533278],
    zoom: 13
});

const geocoder = new MapboxGeocoder({
    accessToken: mapboxgl.accessToken,
    mapboxgl: mapboxgl,
    types: 'address',
    layers: ['address']
});

map.addControl(geocoder, 'top-left');

let address;

map.on('load', () => {
    // Listen for the `geocoder.input` event that is triggered when a user
    // makes a selection
    geocoder.on('result', (event) => {
        let address2 = JSON.stringify(event.result, null, 2);
        console.log(address2);
        address = event.result;
    });
});

window.onload = function () {

    let btn = document.getElementById("btnSave");

    btn.onclick = function () {

        try {
            if (address != null) {

                let { text, geometry, context } = address;
                let addressCode = context[0].text;
                let city = context[1].text;
                let district = context[2].text;
                let country = context[3].text;
                let coord = geometry.coordinates;

                $.ajax({
                    type: 'POST',
                    url: '/MyClub/ReceveAddress',
                    dataType: 'json',
                    data: {
                        CoordinateY: coord[1],
                        CoordinateX: coord[0],
                        ZipCode: addressCode,
                        Street: text,
                        City: city,
                        District: district,
                        Country: country,
                    },
                   
                }).done(function () {
                    console.log("Sucessosssssssssss")
                    alert("Sucesso");
                    btn.disabled = true;
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    console.log("Erro: " + textStatus + ", " + errorThrown);
                    console.log("Resposta do servidor: " + jqXHR.responseText);
                });
            }
        } catch (error) {
            alert("Terá de inserir uma localização com rua incluida");
        }
    }
};
