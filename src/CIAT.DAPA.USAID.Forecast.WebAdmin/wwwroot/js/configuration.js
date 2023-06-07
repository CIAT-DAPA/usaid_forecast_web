﻿$(document).ready(function () {
    var index = 0;
    var index1 = 0;
    var index2 = 0;
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
    $("#add_tgtii").click(function () {
        index1 += 1;
        var row = '<tr>' +
            '<td><input class="form-control" id="tgtii_value_' + index1 + '" name="tgtii_value_' + index1 + '" type="text" value=""></td>' +
            '</tr>';
        $("#tgtii_table tbody").append(row);
    });
    $("#add_tgtff").click(function () {
        index2 += 1;
        var row = '<tr>' +
            '<td><input class="form-control" id="tgtff_value_' + index2 + '" name="tgtff_value_' + index2 + '" type="text" value=""></td>' +
            '</tr>';
        $("#tgtff_table tbody").append(row);
    });
});