window.onload = (event) => {
    loadMap();
};

mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';
let map = new mapboxgl.Map({
    container: 'map', // container ID
    // Choose from Mapbox's core styles, or make your own style with Mapbox Studio
    style: 'mapbox://styles/mapbox/streets-v12', // style URL
    zoom: 10, // starting zoom
    center: [-77.432, 25.0306] // starting position,
});


function loadMap() {

    map.on('load', () => {
        map.resize();
        // Load an image from an external URL.
        map.loadImage(
            'https://docs.mapbox.com/mapbox-gl-js/assets/cat.png',
            (error, image) => {
                if (error) throw error;

                // Add the image to the map style.
                map.addImage('cat', image);

                // Add a data source containing one point feature.
                map.addSource('point', {
                    'type': 'geojson',
                    'data': {
                        'type': 'FeatureCollection',
                        'features': [
                            {
                                'type': 'Feature',
                                'geometry': {
                                    'type': 'Point',
                                    'coordinates': [-77.4144, 25.0759]
                                }
                            }
                        ]
                    }
                });

                // Add a layer to use the image to represent the data.
                map.addLayer({
                    'id': 'points',
                    'type': 'symbol',
                    'source': 'point', // reference the data source
                    'layout': {
                        'icon-image': 'cat', // reference the image
                        'icon-size': 0.25
                    }
                });
            }
        );
    });
}

