// Load the map.
function loadMapScenario() {
    var map = new Microsoft.Maps.Map(document.getElementById('myMap'), {
        credentials: 'AoJqoJIUGkHJa_PyKKY6Bfmq8csIOizScrqoo53GElotN2XfQecO8ExsN4y2NJXV',
        mapTypeId: Microsoft.Maps.MapTypeId.road,
        zoom: 5
    });

    // Create the infobox for the pushpin.
    var infobox = null;
    var infoboxTemplate = '<div class="customInfobox"><img src="https://www.bingmapsportal.com/Content/images/poi_custom.png" align="left" style="margin-right:5px;"/><pre><div class="title">{infoTitle}</div>{infoDescription}</div></pre>';

    //Declare addMarker function
    function addMarker(latitude, longitude, title, activity, mail, pid) {
        var marker = new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(latitude, longitude), { color: 'green' });

        //A title and description for the infobox.
        var infoTitle = title
        var infoDescription = 'Activity Hours: {activity} <br/> Email Address: {mail}.';
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