

async function initWFSOverlays() {
    console.log("initWFSOverlays ")
    try {
        var capabilities = await get_geoserver_wfs_capabilities();

        console.log("wfs capabilities", capabilities)
      

    } catch (e) {
        console.error(e);
        $('#errorMsg').html("An error happened. Please try again later");

    }

}

// load getcapabilities content from geoserver
async function get_geoserver_wfs_capabilities() {
    var res = await axios.get(geoserver_url + "?service=wfs&request=GetCapabilities&version=1.3.0");
    var x2js = new X2JS();
    return x2js.xml_str2json(res.data);
}
