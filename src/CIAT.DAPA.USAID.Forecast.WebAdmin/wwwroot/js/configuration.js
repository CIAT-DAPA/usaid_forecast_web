$(document).ready(function () {
    var index = 0;
    $("#add_region").click(function () {
        index += 1;
        var row = '<tr>' +
                    '<td><input class="form-control" id="left_lower_' + index + '_lat" name="left_lower_' + index + '_lat" type="text" value=""></td>' +
                    '<td><input class="form-control" id="left_lower_' + index + '_lon" name="left_lower_' + index + '_lon" type="text" value=""></td>' +
                    '<td><input class="form-control" id="right_upper_' + index + '_lat" name="right_upper_' + index + '_lat" type="text" value=""></td>' +
                    '<td><input class="form-control" id="right_upper_' + index + '_lon" name="right_upper_' + index + '_lon" type="text" value=""></td>' +
                '</tr>';
        $("#conf_regions tbody").append(row);
    });
});