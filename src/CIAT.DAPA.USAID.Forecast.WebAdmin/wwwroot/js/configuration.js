$(document).ready(function () {
    var index = 0;
    var index1 = 0;
    var index2 = 0;

    let ref = window.location.href
    if (ref.includes("/State/Configuration/")) {
        let button = document.getElementById("add_region");
        button.disabled = true;
        $.ajax({
            url: "/State/ObtainPredictorData",
            type: "GET",

            success: function (listData) {
                button.disabled = false;
                let predictor = "";
                for (let i = 0; i < listData.length; i++) {
                    let option =
                        '<option value="' +
                        listData[i].id +
                        '">' +
                        listData[i].name +
                        "</option>";
                    predictor += option;


                }

                $("#add_region").click(function () {
                    index += 1;
                    var row =
                        "<tr>" +
                        '<td><select class="form-control col-md-6" id="predictor_' +
                        index +
                        '" name="predictor_' +
                        index +
                        '">' +
                        predictor +
                        "</select></td>" +
                        '<td><input class="form-control" id="left_lower_' +
                        index +
                        '_lat" name="left_lower_' +
                        index +
                        '_lat" type="text" value=""></td>' +
                        '<td><input class="form-control" id="left_lower_' +
                        index +
                        '_lon" name="left_lower_' +
                        index +
                        '_lon" type="text" value=""></td>' +
                        '<td><input class="form-control" id="right_upper_' +
                        index +
                        '_lat" name="right_upper_' +
                        index +
                        '_lat" type="text" value=""></td>' +
                        '<td><input class="form-control" id="right_upper_' +
                        index +
                        '_lon" name="right_upper_' +
                        index +
                        '_lon" type="text" value=""></td>' +
                        "</tr>";
                    $("#conf_regions tbody").append(row);
                });
            },
        });
    }



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




    let forcTypeSelect = $("#forc_type");


    let trimesterContainer = $("#trimester_container");
    let bimonthlyContainer = $("#bimonthly_container");

    forcTypeSelect.on("change", function () {
        let selectedValue = $(this).val();

        if (selectedValue === "0") {
            trimesterContainer.show();
            bimonthlyContainer.hide();
        } else if (selectedValue === "1") {
            trimesterContainer.hide();
            bimonthlyContainer.show();
        } else {
            trimesterContainer.show();
            typeContainer.hide();
        }
    });

});