$(function () {
    $('form').submit(function (e) {
        e.preventDefault();

        var query = $('#query').val();

        $.ajax({
            // method : 'post',
            url: '/Branches/Search',
            data: { 'query': query }
        }).done(function (data) {
            $('tbody').html('');

            var template = $('#hidden-template').html();

            $.each(data, function (i, val) {

                var temp = template;

                $.each(val, function (key, value) {
                    temp = temp.replaceAll('{' + key + '}', (value == null ? "Currently there are no activity hours for this branch" : value));
                });

                $('tbody').append(temp);
            });
        });
    });
});

var serchReault, map, searchManager; 

// Load the map.
function loadMapScenario() { 
    map = new Microsoft.Maps.Map(document.getElementById('myMap'), {
        credentials: 'AoJqoJIUGkHJa_PyKKY6Bfmq8csIOizScrqoo53GElotN2XfQecO8ExsN4y2NJXV',
        mapTypeId: Microsoft.Maps.MapTypeId.road,
        zoom: 5
    });

    // Create the infobox for the pushpin.
    var infobox = null;
    var infoboxTemplate = '<div class="customInfobox"><img src="https://www.bingmapsportal.com/Content/images/poi_custom.png" align="left" style="margin-right:5px;"/><div class="title">{infoTitle}</div><pre>{infoDescription}</pre></div>';

    //Declare addMarker function
    function addMarker(latitude, longitude, title, activity, mail, pid) {
        var marker = new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(latitude, longitude), { color: 'green' });

        // If the activity time is empty - show a massage instade.
        if (activity == null) {
            activity = "Currently there are no activity hours for this branch"
        }

        //A title and description for the infobox.
        var infoTitle = title
        var infoDescription = 'Activity Hours: <br/> {activity} <br/> Email Address: {mail}.';
        var content = infoboxTemplate.replace('{infoTitle}', infoTitle).replace('{infoDescription}', infoDescription).replace('{activity}', activity).replace('{mail}', mail)

        infobox = new Microsoft.Maps.Infobox(marker.getLocation(), { visible: false });

        marker.metadata = {
            id: pid,
            description: content
        };

        Microsoft.Maps.Events.addHandler(marker, 'mouseout', hideInfobox);
        Microsoft.Maps.Events.addHandler(marker, 'mouseover', showInfobox);

        infobox.setMap(map);
        map.entities.push(marker);
        marker.setOptions({ enableHoverStyle: true });
    };

    function showInfobox(e) {
        //Make sure the infobox has metadata to display.
        if (e.target.metadata) {
            //Set the infobox options with the metadata of the pushpin.
            infobox.setOptions({
                location: e.target.getLocation(),
                title: e.target.metadata.title,
                description: e.target.metadata.description,
                visible: true
            });
        }
    }

    function hideInfobox(e) {
        infobox.setOptions({ visible: false });
    }

    //add markers to map
    if (branchesList != null) {
        branchesList.forEach(item => addMarker(item.locationLatitude, item.locationLongitude, item.branchName, item.activityTime, item.email, item.id));
    }
}

function SearchAdress() {
    if (!searchManager) {
        //Create an instance of the search manager and perform the search.
        Microsoft.Maps.loadModule('Microsoft.Maps.Search', function () {
            searchManager = new Microsoft.Maps.Search.SearchManager(map);
            SearchAdress()
        });
    } else {
        //Remove any previous results from the map.
        map.entities.clear();

        //Get the users query and geocode it.
        var query = document.getElementById('searchTbx').value;
        geocodeQuery(query);
    }
}

function geocodeQuery(query) {
    var searchRequest = {
        where: query,
        callback: function (r) {
            $('#message').html('');

            // If there is any results: 
            if (r && r.results && r.results.length > 0) {
                // Add the pin to the map
                map.entities.push(new Microsoft.Maps.Pushpin(r.results[0].location, {
                    text: r.results[0].name  + ''
                }));
        
                // Get the Location Latitude
                document.getElementById('LocationLatitude').value = r.results[0].location.latitude;

                // Get the Location Longitude
                document.getElementById('LocationLongitude').value = r.results[0].location.longitude;

                //Determine a bounding box to best view the results.
                var bounds = r.results[0].bestView;

                map.setView({ bounds: bounds });
            }
        },
        errorCallback: function (e) {
            //If there is an error, alert the user about it.
            $('#message').html('No results found.').css('color', 'red');
        }
    };

    //Make the geocode request.
    searchManager.geocode(searchRequest);
}

function GetBranchAddress() {
    map = new Microsoft.Maps.Map('#myMap', {
        credentials: 'AoJqoJIUGkHJa_PyKKY6Bfmq8csIOizScrqoo53GElotN2XfQecO8ExsN4y2NJXV',
        center: new Microsoft.Maps.Location(locationLatitude, locationLongitude),
        zoom: 11
    });

    //Make a request to reverse geocode the center of the map.
    reverseGeocode();
}

function reverseGeocode() {
    //If search manager is not defined, load the search module.
    if (!searchManager) {
        //Create an instance of the search manager and call the reverseGeocode function again.
        Microsoft.Maps.loadModule('Microsoft.Maps.Search', function () {
            searchManager = new Microsoft.Maps.Search.SearchManager(map);
            reverseGeocode();
        });
    } else {
        var searchRequest = {
            location: map.getCenter(),
            callback: function (r) {
                //Print the branch address
                $('#branchAddress').html(r.name);

                //Determine a bounding box to best view the results.
                map.entities.push(new Microsoft.Maps.Pushpin(map.getCenter(), {
                    text: ''
                }));
            },
            errorCallback: function (e) {
                //If there is an error, alert the user about it.
                $('#branchAddress').html('Unable to reverse geocode location.').css('color', 'red');
            }
        };

        //Make the reverse geocode request.
        searchManager.reverseGeocode(searchRequest);
    }
}