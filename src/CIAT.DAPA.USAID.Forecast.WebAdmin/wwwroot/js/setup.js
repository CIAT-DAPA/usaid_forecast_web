$(document).ready(function () {
    var index = 0;
    $("#add_file").click(function () {
        index += 1;
        var row = '<tr><td><input class="form-control" id="name_f_' + index + '" name="name_f_' + index + '" type="text" value=""></td><td><input id="conf_file' + index + '" name="conf_file' + index + '" type="file" class="file" /></td></tr>';
        $("#conf_files tbody").append(row);
    });
});