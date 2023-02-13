
mapboxgl.accessToken = 'pk.eyJ1IjoiZXhhbXBsZXMiLCJhIjoiY2p0MG01MXRqMW45cjQzb2R6b2ptc3J4MSJ9.zA2W0IkI0c6KaAhJfk9bWg';
const map = new mapboxgl.Map({
    container: 'map',
    style: 'mapbox://styles/mapbox/streets-v12',
    center: [-8.896442, 38.533278],
    zoom: 13
});

const geocoder = new MapboxGeocoder({
    accessToken: mapboxgl.accessToken,
    mapboxgl: mapboxgl
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
        console.log("Endereço: " + address[0]);
        //let { text, coordinates, context } = JSON.stringify(event.result, null, 2);
        //let addressCode = context[0];
        //let city = context[1];
        //let district = context[2];
        //let country = context[3];

        //console.log("Endereço: " + address);

        //fetch("https://localhost:7111/MyClub/ReceveAddress", method: "POST",
        //    body: {
        //    ZipCode: addressCode.text,
        //    City: city.text,
        //    District: district.text,
        //    Country: country.text,
        //    CoordinateX: coordinates[0],
        //    CoordinateY: coordinates[1],
        //    Street: text
        //});


        //var newAddress = JsonConvert.DeserializeObject < Address > (address);
        //axios.post('/api/ReceveAddress', newAddress)
        //    .then(function (response) {
        //        console.log(response);
        //    })
        //    .catch(function (error) {
        //        console.error(error);
        //    });

    });
});

window.onload = function () {

    let btn = document.getElementById("btnSave");

    btn.onclick = function () {

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
                    success: console.log("Bacano"),
                    error: function (ex) {
                        console.log('Failed to retrieve counties.' + ex);
                    }
                });
            

            //fetch("https://localhost:7111/MyClub/ReceveAddress", {
            //    method: 'post',
            //    body: {
            //        CoordinateY: coord[1],
            //        CoordinateX: coord[0],
            //        ZipCode: addressCode.text,
            //        Street: text,
            //        City: city.text,
            //        District: district.text,
            //        Country: country.text,
                    
                    
                    
            //    },
            //    headers: {
            //        'Accept': 'application/json',
            //        'Content-Type': 'application/json'
            //    }
            //}).then((response) => {
            //    return response.json()
            //}).then((res) => {
            //    if (res.status === 201) {
            //        console.log("Post successfully created!")
            //    }
            //}).catch((error) => {
            //    console.log(error)
            //})

        }
        

        //fetch("https://localhost:7111/MyClub/ReceveAddress", method: "POST",
        //    body: {
        //    ZipCode: addressCode.text,
        //    City: city.text,
        //    District: district.text,
        //    Country: country.text,
        //    CoordinateX: coordinates[0],
        //    CoordinateY: coordinates[1],
        //    Street: text
        //});

        
    }
};
