$(document).ready(function () {
    let index = 0;
    let tbody = document.getElementById("conf_url").getElementsByTagName("tbody")[0];
    if (tbody.innerHTML != "") {
        let tr = tbody.getElementsByTagName("tr")[tbody.getElementsByTagName("tr").length - 1]
        let input_id = tr.getElementsByTagName("td")[0].getElementsByTagName("input")[0].id
        index = parseInt(input_id.split("_")[input_id.split("_").length - 1])

        let allTrs = tbody.getElementsByTagName("tr")

        for (let i = 1; i <= allTrs.length; i++) {
            $(`#remove_${i}`).click(function (e) {
                e.preventDefault();
                this.parentNode.parentNode.remove();
                index = index - 1

                for (let y = 0; y < allTrs.length; y++) {
                    let tds = allTrs[y].getElementsByTagName("td")
                    for (let z = 0; z < tds.length; z++) {
                        let td_change = tds[z].getElementsByTagName("input")[0] || tds[z].getElementsByTagName("select")[0]
                        let id = td_change.id.split("_")[td_change.id.split("_").length - 1]
                        td_change.id = td_change.id.replace(id, y+1)
                        td_change.name = td_change.name.replace(id, y + 1)
                    }
                }
            })
        }
        
    }

    


    $.ajax({
        url: "/Url/GetSelectData", // Ruta de la acción en el controlador que devuelve los datos
        type: 'GET',
        success: function (listData) {
            let forcTypeOptions = "";
            for (let i = 0; i < listData.forc_type.length; i++) {
                let option = '<option value="' + listData.forc_type[i].id + '">' + listData.forc_type[i].name + '</option>';
                forcTypeOptions += option;
            }

            // Generar opciones para prob_type
            let probTypeOptions = "";
            for (let i = 0; i < listData.prob_type.length; i++) {
                let option = '<option value="' + listData.prob_type[i].id + '">' + listData.prob_type[i].name + '</option>';
                probTypeOptions += option;
            }
            $("#add_url").click(function () {
                index += 1;
                let row = '<tr>' +
                    '<td><input class="form-control col-md-6" id="url_name_' + index + '" name="url_name_' + index + '" type="text" value=""></td>' +
                    '<td><input class="form-control col-md-6" id="url_value_' + index + '" name="url_value_' + index + '" type="text" value=""></td>' +
                    '<td><select class="form-control col-md-6" id="forc_type_' + index + '" name="forc_type_' + index + '">' + forcTypeOptions + '</select></td>' +
                    '<td><select class="form-control col-md-6" id="prob_type_' + index + '" name="prob_type_' + index + '">' + probTypeOptions + '</select></td>' +
                    '<td><input type="button" id="remove_' + index + '" value="Remove" class="btn btn-danger" /></td>' +
                    '</tr>';
                $("#conf_url tbody").append(row);
                $(`#remove_${index}`).click(function (e) {
                    e.preventDefault();
                    this.parentNode.parentNode.remove();
                    index = index - 1

                    let allTrs = tbody.getElementsByTagName("tr")

                    for (let y = 0; y < allTrs.length; y++) {
                        let tds = allTrs[y].getElementsByTagName("td")
                        for (let z = 0; z < tds.length; z++) {
                            let td_change = tds[z].getElementsByTagName("input")[0] || tds[z].getElementsByTagName("select")[0]
                            let id = td_change.id.split("_")[td_change.id.split("_").length - 1]
                            td_change.id = td_change.id.replace(id, y + 1)
                            td_change.name = td_change.name.replace(id, y + 1)
                        }
                    }
                })
            });

        }
    });

});