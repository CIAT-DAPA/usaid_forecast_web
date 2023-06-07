function change_searchbox(placeholder) {
    var menu_country_cbo = $('#menu_country_cbo').select2({ placeholder: placeholder });
    menu_country_cbo.on("change", function (e) {
        var re = /^https?:\/\/[^/]+/;
        window.location.href = re.exec(window.location.href)[0] + "/" + $('#menu_country_cbo').val();
    });

    var menu_climate_cbo = $('#menu_climate_cbo').select2({ placeholder: placeholder });
    menu_climate_cbo.on("change", function (e) {
        var re = /^https?:\/\/[^/]+/;
        window.location.href = re.exec(window.location.href)[0] + "/" + $('#menu_climate_cbo').val();
    });

    var menu_rice_cbo = $('#menu_rice_cbo').select2({ placeholder: placeholder });
    menu_rice_cbo.on("change", function (e) {
        var re = /^https?:\/\/[^/]+/;
        window.location.href = re.exec(window.location.href)[0] + "/" + $('#menu_rice_cbo').val();
    });
    var menu_maize_cbo = $('#menu_maize_cbo').select2({ placeholder: placeholder });
    menu_maize_cbo.on("change", function (e) {
        var re = /^https?:\/\/[^/]+/;
        window.location.href = re.exec(window.location.href)[0] + "/" + $('#menu_maize_cbo').val();
    });
}
$('ul.dropdown-menu').on('click', function (event) {
    var events = $._data(document, 'events') || {};
    events = events.click || [];
    for (var i = 0; i < events.length; i++) {
        if (events[i].selector) {

            //Check if the clicked element matches the event selector
            if ($(event.target).is(events[i].selector)) {
                events[i].handler.call(event.target, event);
            }

            // Check if any of the clicked element parents matches the 
            // delegated event selector (Emulating propagation)
            $(event.target).parents(events[i].selector).each(function () {
                events[i].handler.call(this, event);
            });
        }
    }
    event.stopPropagation(); //Always stop propagation
});