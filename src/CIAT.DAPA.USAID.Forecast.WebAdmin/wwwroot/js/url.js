$(document).ready(function () {
    var index = 0;
    $("#add_url").click(function () {
        index += 1;
        var row = '<tr>' +
            '<td><input class="form-control col-md-6" id="url_name_' + index + '" name="url_name_' + index + '" type="text" value=""></td>' +
            '<td><input class="form-control col-md-6" id="url_value_' + index + '" name="url_value_' + index + '" type="text" value=""></td>' +
                '</tr>';
        $("#conf_url tbody").append(row);
    });
});