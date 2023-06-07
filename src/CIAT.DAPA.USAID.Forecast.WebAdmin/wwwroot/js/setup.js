$(document).ready(function () {
    var index = 0;
    $("#add_file").click(function () {
        index += 1;
        var row = '<tr><td><input class="form-control" id="name_f_' + index + '" name="name_f_' + index + '" type="text" value=""></td><td><input id="conf_file' + index + '" name="conf_file' + index + '" type="file" class="file" /></td></tr>';
        $("#conf_files tbody").append(row);
    });
    $("#crop").change(() => {
        $.get("/Setup/GetCultivarsByIdCrop", { cropId: $("#crop").val() }, (data) => {
            $("#cultivar").empty();
            $.each(data, (index, row) => {
                $("#cultivar").append("<option value='" + row.id + "'>" + row.name + "</option>");
            });
        });
        $.get("/Setup/GetSoilsByIdCrop", { cropId: $("#crop").val() }, (data) => {
            $("#soil").empty();
            $.each(data, (index, row) => {
                $("#soil").append("<option value='" + row.id + "'>" + row.name + "</option>");
            });
        });
    });
});

function deleteFileConf(idConf, idSetup) {
    $.ajax({
        type: "POST",
        url: "/Setup/deleteFileConf",
        data: { idConf: idConf, idSetup: idSetup },
        success: (result) => {
            console.log(result);
            if (result) {
                window.location.reload();
            }
        },
        error: (req, status, error) => { console.log(req); console.log(status); console.log(error); }
    });
}