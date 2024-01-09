var InitialProjectDropdownList, InitialJobCategories;
var initial_job_category_row,initial_item_type_row,initial_item_row, append_job_category_row;
var initail_table_footer;
var category_code, category_description;

/* start initial control */
InitialProjectDropdownList=function(project_id) {
    $.ajax({
        url: '/Project/ProjectDropdownList',
        type: "get",
        dataType: "json",
        success: function (data) {
            $('#project_id').empty();
            $('#project_id').append("" +
                "<option selected disabled>--- select Project ---</option>"
            );
            $.each(data, function (index, item) {
                $.each(item, function (index1, item1) {
                    $('#project_id').append("" +
                            "<option value='" + item1.project_id + "'>" + item1.project_full_name + "</option>"
                        );
                });
            });
            if (project_id != undefined || project_id != null || project_id != "")
                $('#project_id').val(project_id);
                
        },
        error: function (data) {
            alert('fail');
        }
    });
}

InitialJobCategories=function(ind, first) {
    $.ajax({
        url: '/JobCategory/JobCategoriesDropdownList',
        type: "get",
        dataType: "json",
        success: function (data) {
            if (first == undefined || first == null || first == "") {
                $.each(data, function (index, item) {
                    $.each(item, function (index1, item1) {
                        $('#boq_table tbody tr').eq(ind + 1).find('select.job_category_description').append("" +
                                "<option value='" + item1.j_category_id + "'>" + item1.j_category_name + "</option>"
                            );
                    });
                });
            } else {
                $.each(data, function (index, item) {
                    $.each(item, function (index1, item1) {
                        $('#boq_table tbody tr:first-child').find('select.job_category_description').append("" +
                                "<option value='" + item1.j_category_id + "'>" + item1.j_category_name + "</option>"
                            );
                    });
                });
            }
        },
        error: function (data) {
            alert('fail');
        }
    });
}

/*  end initial control  */

initail_table_footer=function(amount) {
    $('#boq_table tbody').find('tr:last-child').after(
            '<tr>' +
                '<td colspan="7" style="text-align:right !important;"><label class="control-label">Grand Total</label></td>' +
                '<td><label class="control-label" id="sub_total">' + parseFloat(amount).toFixed(2) + '</label></td>' +
                '<td></td>' +
                '<td></td>' +
                '<td></td>' +
                '<td></td>' +
                '<td></td>' +
                '<td></td>' +
                '<td></td>' +
            '</tr>'
        );
}

initial_job_category_row = function (index, row_no, id_no, job_category_code, first) {
    var roman_number = String.fromCharCode(8543 + Number(row_no));
    if (job_category_code == undefined)
        job_category_code = "";
    if (first == undefined || first == null || first == "") {
        $('#boq_table tbody').find('tr').eq(index).after(
            "<tr class='job" + id_no + "' id='job" + id_no + "'>" +
                "<td>" + roman_number + "<input type='hidden' class='job_id' id='j" + id_no + "'/></td>" +
                "<td><label class='job_category_code'>" + job_category_code + "</label></td>" +
                "<td colspan='5'><select class='form-control job_category_description'><option selected disabled value=''>Select job category</option></select></td>" +
                "<td><input type='number' class='form-control job_category_amount' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_total_amount_by_job_category()'/></td>" +
                "<td>" +
                    "<select class='form-control job_remark'>" +
                        "<option selected disabled value=''>Select remark</option>" +
                        "<option value='In Source'>In Source</option>" +
                        "<option value='Out Source'>Out Source</option>" +
                    "</select>" +
                "</td>" +
                "<td></td>" +
                "<td></td>" +
                "<td></td>" +
                "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_type_row(this,true)'><i class='fa fa-plus-square' aria-hidden='true'></i></a></td>" +
                "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_job_category_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
                "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_job_category_row(this)'><i class='fa fa-plus-circle' aria-hidden='true'></i></a></td>" +
            "</tr>"
        );
    } else {
        $('#boq_table').append(
            "<tr class='job" + id_no + "' id='job" + id_no + "'>" +
                "<td>" + roman_number + "<input type='hidden' class='job_id' id='j" + id_no + "'/></td>" +
                "<td><label class='job_category_code'>" + job_category_code + "</label></td>" +
                "<td colspan='5'><select class='form-control job_category_description'><option selected disabled value=''>Select job category</option></select></td>" +
                "<td><input type='number' class='form-control job_category_amount' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_total_amount_by_job_category()'/></td>" +
                "<td>" +
                    "<select class='form-control job_remark'>" +
                        "<option selected disabled value=''>Select remark</option>" +
                        "<option value='In Source'>In Source</option>" +
                        "<option value='Out Source'>Out Source</option>" +
                    "</select>" +
                "</td>" +
                "<td></td>" +
                "<td></td>" +
                "<td></td>" +
                "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_type_row(this,true)'><i class='fa fa-plus-square' aria-hidden='true'></i></a></td>" +
                "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_job_category_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
                "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_job_category_row(this)'><i class='fa fa-plus-circle' aria-hidden='true'></i></a></td>" +
            "</tr>"
        );
    }
    InitialJobCategories(index, first);
}

//initial_item_type_row=function(index, row_no, type_no, row_letter, first) {
//    if (first == undefined || first == null || first == "") {
//        $('#boq_table tbody').find('tr').eq(index + 1).after("" +
//                "<tr class='type" + row_no + type_no + " job" + row_no + "' id='type" + row_no + type_no + "'>" +
//                    "<td style='padding-left:25px !important;'><input type='hidden' class='item_type_id' id='" + row_no + type_no + "'>" + row_letter + "</td>" +
//                    "<td><input type='text' class='form-control item_type_code' id='itemType"+row_no+type_no+"' onfocus='get_item_type_by_code(this)'/></td>" +
//                    "<td><input type='text' class='form-control item_type_description' onchange='get_item_type_by_description(this)'/></td>" +
//                    "<td colspan='4'><label class='chart-account'></label></td>" +
//                    "<td><input type='number' class='form-control type_amount' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_total_amount_by_type(this)'/></td>" +
//                    "<td>" +
//                        "<select class='form-control remark' onchange='InitialRemark(this)'>" +
//                            "<option selected disabled value=''>Select remark</option>" +
//                            "<option value='In Source'>In Source</option>" +
//                            "<option value='Out Source'>Out Source</option>" +
//                        "</select>" +
//                    "</td>" +
//                    "<td></td>" +
//                    "<td></td>" +
//                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_type_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
//                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_type_row(this,false)'><i class='fa fa-plus-square' aria-hidden='true'></i></a></td>" +
//                    "<td></td>" +
//                    "<td></td>" +
//                "</tr>"
//            );
//    }
//    else if (first == "false") {
//        $('#boq_table').find('tr').eq(index).after("" +
//                "<tr class='type" + row_no + type_no + " job" + row_no + "' id='type" + row_no + type_no + "'>" +
//                    "<td style='padding-left:25px !important;'><input type='hidden' class='item_type_id' id='" + row_no + type_no + "'>" + row_letter + "</td>" +
//                    "<td><input type='text' class='form-control item_type_code' id='itemType" + row_no + type_no + "' onfocus='get_item_type_by_code(this)'/></td>" +
//                    "<td><input type='text' class='form-control item_type_description' onchange='get_item_type_by_description(this)'/></td>" +
//                    "<td colspan='4'><label class='chart-account'></label></td>" +
//                    "<td><input type='number' class='form-control type_amount' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_total_amount_by_type(this)'/></td>" +
//                    "<td>" +
//                        "<select class='form-control remark' onchange='InitialRemark(this)'>" +
//                            "<option selected disabled value=''>Select remark</option>" +
//                            "<option value='In Source'>In Source</option>" +
//                            "<option value='Out Source'>Out Source</option>" +
//                        "</select>" +
//                    "</td>" +
//                    "<td></td>" +
//                    "<td></td>" +
//                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_type_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
//                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_type_row(this,false)'><i class='fa fa-plus-square' aria-hidden='true'></i></a></td>" +
//                    "<td></td>" +
//                    "<td></td>" +
//                "</tr>"
//            );
//    }
//    else {
//        $('#boq_table').append("" +
//                 "<tr class='type" + row_no + type_no + " job" + row_no + "' id='type" + row_no + type_no + "'>" +
//                    "<td style='padding-left:25px !important;'><input type='hidden' class='item_type_id' id='" + row_no + type_no + "'>" + row_letter + "</td>" +
//                    "<td><input type='text' class='form-control item_type_code' id='itemType" + row_no + type_no + "' onfocus='get_item_type_by_code(this)'/></td>" +
//                    "<td><input type='text' class='form-control item_type_description' onchange='get_item_type_by_description(this)'/></td>" +
//                    "<td colspan='4'><label class='chart-account'></label></td>" +
//                    "<td><input type='number' class='form-control type_amount' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_total_amount_by_type(this)'/></td>" +
//                    "<td>" +
//                        "<select class='form-control remark' onchange='InitialRemark(this)'>" +
//                            "<option selected disabled value=''>Select remark</option>" +
//                            "<option value='In Source'>In Source</option>" +
//                            "<option value='Out Source'>Out Source</option>" +
//                        "</select>" +
//                    "</td>" +
//                    "<td></td>" +
//                    "<td></td>" +
//                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_type_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
//                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_type_row(this,false)'><i class='fa fa-plus-square' aria-hidden='true'></i></a></td>" +
//                    "<td></td>" +
//                    "<td></td>" +
//                "</tr>"
//            );
//    }
//}

initial_item_type_row = function (index, row_no, type_no, row_letter, first) {
    if (first == undefined || first == null || first == "") {
        $('#boq_table tbody').find('tr').eq(index + 1).after("" +
                "<tr class='type" + row_no + type_no + " job" + row_no + "' id='type" + row_no + type_no + "'>" +
                    "<td style='padding-left:25px !important;'><input type='hidden' class='item_type_id' id='" + row_no + type_no + "'>" + row_letter + "</td>" +
                    "<td><input type='text' class='form-control item_type_code' id='itemType" + row_no + type_no + "' onfocus='get_item_type_by_code(this)'/></td>" +
                    "<td><input type='text' class='form-control item_type_description' onchange='get_item_type_by_description(this)'/></td>" +
                    "<td colspan='4'><label class='chart-account'></label></td>" +
                    "<td><input type='number' class='form-control type_amount' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_total_amount_by_type(this)'/></td>" +
                    "<td>" +
                        "<select class='form-control remark' onchange='InitialRemark(this)'>" +
                            "<option selected disabled value=''>Select remark</option>" +
                            "<option value='In Source'>In Source</option>" +
                            "<option value='Out Source'>Out Source</option>" +
                        "</select>" +
                    "</td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_type_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_type_row(this,false)'><i class='fa fa-plus-square' aria-hidden='true'></i></a></td>" +
                    "<td></td>" +
                    "<td></td>" +
                "</tr>"
            );
    }
    else if (first == "false") {
        $('#boq_table').find('tr').eq(index).after("" +
                "<tr class='type" + row_no + type_no + " job" + row_no + "' id='type" + row_no + type_no + "'>" +
                    "<td style='padding-left:25px !important;'><input type='hidden' class='item_type_id' id='" + row_no + type_no + "'>" + row_letter + "</td>" +
                    "<td><input type='text' class='form-control item_type_code' id='itemType" + row_no + type_no + "' onfocus='get_item_type_by_code(this)'/></td>" +
                    "<td><input type='text' class='form-control item_type_description' onchange='get_item_type_by_description(this)'/></td>" +
                    "<td colspan='4'><label class='chart-account'></label></td>" +
                    "<td><input type='number' class='form-control type_amount' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_total_amount_by_type(this)'/></td>" +
                    "<td>" +
                        "<select class='form-control remark' onchange='InitialRemark(this)'>" +
                            "<option selected disabled value=''>Select remark</option>" +
                            "<option value='In Source'>In Source</option>" +
                            "<option value='Out Source'>Out Source</option>" +
                        "</select>" +
                    "</td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_type_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_type_row(this,false)'><i class='fa fa-plus-square' aria-hidden='true'></i></a></td>" +
                    "<td></td>" +
                    "<td></td>" +
                "</tr>"
            );
    }
    else {
        $('#boq_table').append("" +
                 "<tr class='type" + row_no + type_no + " job" + row_no + "' id='type" + row_no + type_no + "'>" +
                    "<td style='padding-left:25px !important;'><input type='hidden' class='item_type_id' id='" + row_no + type_no + "'>" + row_letter + "</td>" +
                    "<td><input type='text' class='form-control item_type_code' id='itemType" + row_no + type_no + "' onfocus='get_item_type_by_code(this)'/></td>" +
                    "<td><input type='text' class='form-control item_type_description' onchange='get_item_type_by_description(this)'/></td>" +
                    "<td colspan='4'><label class='chart-account'></label></td>" +
                    "<td><input type='number' class='form-control type_amount' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_total_amount_by_type(this)'/></td>" +
                    "<td>" +
                        "<select class='form-control remark' onchange='InitialRemark(this)'>" +
                            "<option selected disabled value=''>Select remark</option>" +
                            "<option value='In Source'>In Source</option>" +
                            "<option value='Out Source'>Out Source</option>" +
                        "</select>" +
                    "</td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_type_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_type_row(this,false)'><i class='fa fa-plus-square' aria-hidden='true'></i></a></td>" +
                    "<td></td>" +
                    "<td></td>" +
                "</tr>"
            );
    }
}

initial_item_row=function(index, row_no, type_no, item_no, row_number, first, remark) {
    if (first == undefined || first == null || first == "" || first == "false") {
        $('#boq_table').find('tr').eq(index).after("" +
                "<tr class='type" + row_no + type_no + " job" + row_no + "' id='item" + row_no + type_no + item_no + "'>" +
                    "<td style='padding-left:50px !important;'><input type='hidden' class='product_id' id='" + row_no + type_no + item_no + "' />" + row_number + "</td>" +
                    "<td><input type='text' class='form-control item_code' onchange='initial_item_by_item_code(this)'/></td>" +
                    "<td><label class='item_description'></label></td>" +
                    "<td></td>" +
                    "<td><label class='item_unit'></label></td>" +
                    "<td><input type='number' class='form-control item_qty' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_amount(this)'/></td>" +
                    "<td><input type='number' class='form-control item_unit_price' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_amount(this)'/></td>" +
                    "<td><label class='item_amount'>" + parseFloat(0).toFixed(2) + "</lable></td>" +
                    "<td><label class='item_remark'>" + remark + "</label></td>" +
                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_item_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_item_row(this)'><span class='glyphicon glyphicon-plus'></span></a></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                "</tr>"
            );
    } else {
        $('#boq_table tbody').append("" +
                "<tr class='type" + row_no + type_no + " job" + row_no + "' id='item" + row_no + type_no + item_no + "'>" +
                    "<td style='padding-left:50px !important;'><input type='hidden' class='product_id' id='" + row_no + type_no + item_no + "' />" + row_number + "</td>" +
                    "<td><input type='text' class='form-control item_code' onchange='initial_item_by_item_code(this)'/></td>" +
                    "<td><label class='item_description'></label></td>" +
                    "<td></td>" +
                    "<td><label class='item_unit'></label></td>" +
                    "<td><input type='number' class='form-control item_qty' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_amount(this)'/></td>" +
                    "<td><input type='number' class='form-control item_unit_price' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_amount(this)'/></td>" +
                    "<td><label class='item_amount'>" + parseFloat(0).toFixed(2) + "</lable></td>" +
                    "<td><label class='item_remark'>" + remark + "</label></td>" +
                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_item_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_item_row(this)'><span class='glyphicon glyphicon-plus'></span></a></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                "</tr>"
            );
    }
}

append_job_category_row=function(row) {
    var job_category_code;
    var index = row.parentNode.parentNode.rowIndex;
    var count_table_row = $('#boq_table tbody tr').length;
    var j_category_code = $('#boq_table tr').eq(index).find('label.job_category_code').text();
    if (j_category_code == null || j_category_code == "") {
        job_category_code = "";
    } else {
        var old_j_catagory_code = j_category_code.split('-');
        var code_number = Number(old_j_catagory_code[old_j_catagory_code.length - 1]) + 1;
        var j_category_last_split = (code_number.toString().length == 1) ? "00" + code_number : (code_number.toString().length == 2) ? "0" + code_number : code_number;
        job_category_code = old_j_catagory_code[0] + "-" + old_j_catagory_code[1] + "-" + j_category_last_split;
    }
    var arr_job_id = new Array();
    for (var i = 1; i <= count_table_row; i++) {
        var id = $('#boq_table tr').eq(i).attr('id');
        if (id != undefined && id.substr(0, 3) == "job")
            arr_job_id.push(id);
    }
    var id_no = arr_job_id[arr_job_id.length - 1];
    id_no = Number(id_no.substr(id_no.length - 1, 1)) + 1;
    var no = Number(arr_job_id.length + 1);
    ind = Number(count_table_row) - 2;
    initial_job_category_row(ind, no, id_no, job_category_code);
    $('#boq_table tr td:nth-child(11)').eq(index - 1).html('');
}

function remove_job_category_row(row) {
    var arr_job_id = new Array();
    var index = row.parentNode.parentNode.rowIndex;
    var job_class = $('#boq_table tr').eq(index).attr('class');
    var job_id = $('#boq_table tr').eq(index).attr('id');
    var job_class_row = $('.' + job_class);
    var count_job_class_row = job_class_row.length;
    var is_last_child = $('#boq_table tbody tr td:nth-child(11)').eq(index - 1).find('a').length == 0 ? false : true;
    var count_table_row = $('#boq_table tbody tr').length;
    var j_category_code = $('#boq_table tr').eq(index).find('label.job_category_code').text();
    var old_j_catagory_code = j_category_code.split('-');
    for (var i = 1; i <= count_table_row; i++) {
        var id = $('#boq_table tr').eq(i).attr('id');
        if (id != undefined && id.substr(0, 3) == "job")
            arr_job_id.push(id);
    }
    if (arr_job_id.length > 1) {

        for (var i = 0; i < count_job_class_row; i++) {
            document.getElementById('boq_table').deleteRow(index);
        }
        if (is_last_child) {
            $('#boq_table tbody tr#' + arr_job_id[arr_job_id.length - 2] + ' td:nth-child(11)').html("<a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_job_category_row(this)'><span class='glyphicon glyphicon-plus'></span></a>");
        }
        else {
            var deleted_index = arr_job_id.indexOf(job_id);
            arr_job_id.splice(deleted_index, 1);
            for (var i = 0; i < arr_job_id.length; i++) {
                var roman_number = String.fromCharCode(8544 + Number(i));
                $('tr#' + arr_job_id[i] + ' td:first-child').html(roman_number);
            }
        }
    }
    calculate_total_amount_by_job_category();
    initial_job_category_code(old_j_catagory_code[1]);
}

function append_type_row(row, is_job) {
    var index = row.parentNode.parentNode.rowIndex;
    var type_class, count_type_class, job_no, type_old_letter, type_new_letter, new_index, row_letter;
    if (is_job) {
        type_class = $('#boq_table tr').eq(index).attr('class');
        job_no = type_class.replace(/[^\d]/g, '');
        row_letter = type_new_letter = "A";
        new_index = index;
    }
    else {
        row_letter = $('#boq_table tr').eq(index).find('td:first-child').text();
        row_letter = String.fromCharCode(Number(row_letter.charCodeAt(0)) + 1);
        type_class = $('#boq_table tr').eq(index).attr('class').split(' ');
        count_type_class = $('.' + type_class[0]).length;
        job_no = type_class[0].replace(/[^\d]/g, '');
        type_old_letter = type_class[0].substr(type_class[0].length - 1, 1);
        type_new_letter = String.fromCharCode(Number(type_old_letter.charCodeAt(0) + 1));
        new_index = index + (count_type_class - 1);
    }
    initial_item_type_row(new_index, job_no, type_new_letter, row_letter, "false");
    if (is_job) {
        $('#boq_table tr td:nth-child(9)').eq(index - 1).html('');
        $('#boq_table tr td:nth-child(4)').eq(index - 1).html('<label class="job_category_amount">' + parseFloat(0).toFixed(2) + '</lable>');
        $('#boq_table tr td:nth-child(5)').eq(index - 1).html('<label class="job_remark"></lable>');
    }
    else
        $('#boq_table tr td:nth-child(10)').eq(index - 1).html('');
    calculate_total_amount_by_job_category();
}

function remove_type_row(row) {
    var arr_type_id = new Array();
    var index = row.parentNode.parentNode.rowIndex;
    var type_class_name = $('#boq_table tr').eq(index).attr('class').split(' ');
    var type_id_name = $('#boq_table tr').eq(index).attr('id');
    var type_class_ele = $('.' + type_class_name[0]);
    var count_type_class_row = type_class_ele.length;
    var job_no = type_class_name[0].replace(/[^\d]/g, '');
    var is_last_child = $('#boq_table tbody tr td:nth-child(9)').eq(index - 1).find('a').length == 0 ? false : true;
    var count_table_row = $('#boq_table tbody tr').length;
    for (var i = 1; i <= count_table_row; i++) {
        var id = $('#boq_table tr').eq(i).attr('id');
        if (id != undefined && id.substr(0, 5) == "type"+job_no)
            arr_type_id.push(id);
    }
    for (var i = 0; i < count_type_class_row; i++) {
        document.getElementById('boq_table').deleteRow(index);
    }
    if (arr_type_id.length == 1) {
        $('#boq_table tbody tr#' + type_class_name[1] + ' td:nth-child(4)').html(
                "<input type='number' class='form-control job_category_amount' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_total_amount_by_job_category()'/>"
            );
        $('#boq_table tbody tr#' + type_class_name[1] + ' td:nth-child(5)').html(
                    "<select class='form-control job_remark'>" +
                        "<option selected disabled value=''>Select remark</option>" +
                        "<option value='In Source'>In Source</option>" +
                        "<option value='Out Source'>Out Source</option>" +
                    "</select>"
            );
        $('#boq_table tbody tr#' + type_class_name[1] + ' td:nth-child(9)').html("<a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_type_row(this,true)'><span class='glyphicon glyphicon-plus'></span></a>");
    }
    else if (is_last_child) {
        $('#boq_table tbody tr#' + arr_type_id[arr_type_id.length - 2] + ' td:nth-child(10)').html("<a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_type_row(this)'><span class='glyphicon glyphicon-plus'></span></a>");
        var deleted_index = arr_type_id.indexOf(type_id_name);
        arr_type_id.splice(deleted_index, 1);
        for (var i = 0; i < arr_type_id.length; i++) {
            var letter = String.fromCharCode(Number(65 + i));
            $('tr#' + arr_type_id[i] + ' td:nth-child(1)').html(letter);
        }
    }
    calculate_total_amount_by_type(row, arr_type_id, type_class_name[1]);
}

function append_item_row(row) {
    var index = row.parentNode.parentNode.rowIndex;
    var item_id_name = $('#boq_table tr').eq(index).attr('id');
    var type_class = $('#boq_table tr').eq(index).attr('class').split(' ');
    var count_type_class = $('.' + type_class[0]).length;
    var job_no = type_class[0].replace(/[^\d]/g, '');
    var type_old_letter = type_class[0].substr(type_class[0].length - 1, 1);
    var item_old_number = Number(item_id_name.substr(5, 5).replace(/[^\d]/g, '')) + 1;
    var remark = $('#boq_table tr').eq(index).find('label.item_remark').text();
    var row_number = $('#boq_table tr').eq(index).find('td:first-child').text();
    row_number = Number(row_number) + 1;
    initial_item_row(index, job_no, type_old_letter, item_old_number, row_number, "false", remark);
    $('#boq_table tr td:nth-child(11)').eq(index - 1).html('');
}

function remove_item_row(row) {
    var is_last_child;
    var arr_id = new Array();;
    var index = row.parentNode.parentNode.rowIndex;
    var Class = $('#boq_table tr').eq(index).attr('class').split(' ');
    var item_id = $('#boq_table tr').eq(index).attr('id');
    var count_item_type = $('.' + Class[0]).length;
    var remove_row = $('#boq_table tbody tr td:nth-child(11)').eq(index - 1).find('a').length;
    if (remove_row == 0)
        is_last_child = false;
    else {
        is_last_child = true;
    }
    var count_table_row = $('#boq_table tbody tr').length;
    for (var i = 1; i <= count_table_row; i++) {
        var id = $('#boq_table tr').eq(i).attr('id');
        if (id != undefined && id.substr(0, 4) == "item")
            arr_id.push(id);
    }
    document.getElementById('boq_table').deleteRow(index);
    if (count_item_type > 1) {
        if (arr_id.length == 1) {
            $('#boq_table tbody tr#' + Class[0] + ' td:nth-child(5)').html(
                "<input type='number' class='form-control type_amount' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_total_amount_by_type(this)'/>"
            );
        } else {
            if (is_last_child) {
                $('#boq_table tr').eq(index - 1).find('td:nth-child(11)').html("<a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_item_row(this)'><span class='glyphicon glyphicon-plus'></span></a>");
            }
        }
        var deleted_index = arr_id.indexOf(item_id);
        arr_id.splice(deleted_index, 1);
        for (var i = 0; i < arr_id.length; i++) {
            $('tr#' + arr_id[i] + ' td:nth-child(1)').html(Number(i + 1));
        }
        calculate_total_amount_by_item(index - 1, arr_id);
    }

}

//function get_item_type_by_code(row) {
//    var category_id;
//    var ind = row.parentNode.parentNode.rowIndex;
//    var item_class = $('#boq_table tr').eq(ind).attr('class').split(' ');
//    var row_id = $('#boq_table tr').eq(ind).find('input[type="text"].item_type_code').attr('id');
//    category_code = $('#boq_table tr').eq(ind).find('input[type="text"].item_type_code').val();
//    var remark = $('#boq_table tr').eq(ind).find('label.type_remark').text();


//    function autocompleted(row_id) {
//        $("#" + row_id).autocomplete({
//            source: '/ProductCategory/GetProductCategoryCodes',
//            select: function (event, ui) {
//                //AutoCompleteSelectHandler(event, ui, ir_id, ind);
//            }
//        });
//    }
//    /*
//    if (category_code != "") {
//        $.ajax({
//            url: '/ProductCategory/GetProductCategoryInfo',
//            type: "get",
//            dataType: "json",
//            data: { category_code: category_code },
//            success: function (data) {
//                $.each(data, function (index, item) {
//                    $('#boq_table tbody tr').eq(ind - 1).find('input[type="hidden"].item_type_id').val(item.p_category_id);
//                    $('#boq_table tbody tr').eq(ind - 1).find('input[type="text"].item_type_description').val(item.p_category_name);
//                    $('#boq_table tbody tr').eq(ind - 1).find('label.chart-account').text(item.chart_account == null ? "" : item.chart_account);
//                    category_id = item.p_category_id;
//                    initial_item_data_row(category_id, ind, item_class[0], remark);
//                });
//            },
//            error: function (XMLHttpRequest, textStatus, errorThrown) {
//                $('#boq_table tbody tr').eq(ind - 1).find('input[type="hidden"].item_type_id').val('');
//                $('#boq_table tbody tr').eq(ind - 1).find('input[type="text"].item_type_description').val('');
//                $('#boq_table tbody tr').eq(ind - 1).find('label.chart-account').text('');
//                alert(textStatus);
//            }
//        });
//    }
//    */
//}

function get_item_type_by_code(row) {
    var category_id;
    var ind = row.parentNode.parentNode.rowIndex;
    var item_class = $('#boq_table tr').eq(ind).attr('class').split(' ');
    var row_id = $('#boq_table tr').eq(ind).find('input[type="text"].item_type_code').attr('id');
    category_code = $('#boq_table tr').eq(ind).find('input[type="text"].item_type_code').val();
    var remark = $('#boq_table tr').eq(ind).find('label.type_remark').text();

    autocompleted(row_id, ind, item_class[0], remark);

    function autocompleted(row_id, ind, item_class, remark) {
        $("#" + row_id).autocomplete({
            source: '/ProductCategory/GetProductCategoryCodes',
            select: function (event, ui) {
                AutoCompleteSelectHandler(event, ui, ind, item_class, remark);
            }
        });
    }

    function AutoCompleteSelectHandler(event, ui, ind, item_class, remark) {
        var selectedObj = ui.item;
        var item = (selectedObj.value).split(' ');
        category_id = selectedObj.id;
        category_code = item[0];
        GetItemTypeDataRow(ind, category_id, item_class, remark)
    }

    function GetItemTypeDataRow(ind, item_type_id, item_class, remark) {
        if (item_type_id != "") {
            $.ajax({
                url: '/ProductCategory/GetProductCategoryInfo',
                type: "get",
                dataType: "json",
                async: false,
                data: { category_id: item_type_id },
                success: function (data) {
                    $.each(data, function (index, item) {
                        $('#boq_table tbody tr').eq(ind - 1).find('input[type="hidden"].item_type_id').val(item.p_category_id);
                        $('#boq_table tbody tr').eq(ind - 1).find('input[type="text"].item_type_code').val(item.p_category_code);
                        $('#boq_table tbody tr').eq(ind - 1).find('input[type="text"].item_type_description').val(item.p_category_name);
                        $('#boq_table tbody tr').eq(ind - 1).find('label.chart-account').text(item.chart_account == null ? "" : item.chart_account);
                        category_id = item.p_category_id;
                        initial_item_data_row(category_id, ind, item_class, remark);
                    });
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $('#boq_table tbody tr').eq(ind - 1).find('input[type="hidden"].item_type_id').val('');
                    $('#boq_table tbody tr').eq(ind - 1).find('input[type="text"].item_type_description').val('');
                    $('#boq_table tbody tr').eq(ind - 1).find('label.chart-account').text('');
                    alert(textStatus);
                }
            });
        }
    }

    function initial_item_data_row(category_id, row_index, item_class, remark) {
        if (category_id != "") {
            var type_letter = item_class.substr(item_class.length - 2, 2);
            var job_no = item_class.replace(/[^\d]/g, '');
            var item_cls = $('.type' + type_letter);
            var str_btn_add = "";
            if (item_cls.length > 0) {
                for (var i = 1; i < item_cls.length; i++) {
                    var idx = item_cls[i].rowIndex;
                    document.getElementById('boq_table').deleteRow(idx);
                }
            }
            $.ajax({
                url: '/Product/GetProductDataRow',
                type: "get",
                dataType: "json",
                async: false,
                data: { category_id: category_id },
                success: function (data) {
                    $.each(data, function (index, item) {
                        if (item.length > 0) {
                            $('#boq_table tr').eq(row_index).find('td:nth-child(5)').html('<label class="type_amount">' + parseFloat(0).toFixed(2) + '</lable>');
                            var arr_type_id = new Array();
                            var count_table_row = $('#boq_table tbody tr').length;
                            for (var i = 1; i <= count_table_row; i++) {
                                var id = $('#boq_table tr').eq(i).attr('id');
                                if (id != undefined && id.substr(0, 5) == "type" + job_no)
                                    arr_type_id.push(id);
                            }
                            calculate_total_amount_by_type("", arr_type_id, job_no);
                        }
                        $.each(item, function (index1, item1) {
                            if (index1 == item.length - 1)
                                str_btn_add = "<a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_item_row(this)'><span class='glyphicon glyphicon-plus'></span></a>";
                            $('#boq_table').find('tr').eq(row_index + index1).after("" +
                                "<tr class='type" + type_letter + " job" + job_no + "' id='item" + type_letter + (Number(index1) + 1) + "'>" +
                                    "<td style='padding-left:50px !important;'><input type='hidden' class='product_id' value='" + item1.product_id + "' id='" + type_letter + (Number(index1) + 1) + "' />" + Number(index1 + 1) + "</td>" +
                                    "<td><input type='text' class='form-control item_code' value='" + item1.product_code + "' onchange='initial_item_by_item_code(this)'/></td>" +
                                    "<td><label class='item_description'>" + item1.product_name + "</label></td>" +
                                    "<td></td>" +
                                    "<td><label class='item_unit'>" + item1.product_unit + "</label></td>" +
                                    "<td><input type='number' class='form-control item_qty' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_amount(this)'/></td>" +
                                    "<td><input type='number' class='form-control item_unit_price' value='" + parseFloat(item1.unit_price).toFixed(2) + "' onchange='calculate_amount(this)'/></td>" +
                                    "<td><label class='item_amount'>" + parseFloat(0).toFixed(2) + "</lable></td>" +
                                    "<td><label class='item_remark'>" + remark + "</label></td>" +
                                    "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_item_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
                                    "<td>" + str_btn_add + "</td>" +
                                    "<td></td>" +
                                    "<td></td>" +
                                    "<td></td>" +
                                    "<td></td>" +
                                "</tr>"
                            );
                        });
                        calculate_total_amount_by_job_category();
                    });
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus);
                }
            });
        }
    }
}

function get_item_type_by_description(row) {

}

function initial_item_data_row(category_id, row_index, item_class, remark) {
    if (category_id != "") {
        var type_letter = item_class.substr(item_class.length - 2, 2);
        var job_no = item_class.replace(/[^\d]/g, '');
        var item_cls = $('.type' + type_letter);
        var str_btn_add = "";
        if (item_cls.length > 0) {
            for (var i = 1; i < item_cls.length; i++) {
                var idx = item_cls[i].rowIndex;
                document.getElementById('boq_table').deleteRow(idx);
            }
        }
        $.ajax({
            url: '/Product/GetProductDataRow',
            type: "get",
            dataType: "json",
            data: { category_id: category_id },
            success: function (data) {
                $.each(data, function (index, item) {
                    if (item.length > 0) {
                        $('#boq_table tr').eq(row_index).find('td:nth-child(5)').html('<label class="type_amount">' + parseFloat(0).toFixed(2) + '</lable>');
                        var arr_type_id = new Array();
                        var count_table_row = $('#boq_table tbody tr').length;
                        for (var i = 1; i <= count_table_row; i++) {
                            var id = $('#boq_table tr').eq(i).attr('id');
                            if (id != undefined && id.substr(0, 5) == "type" + job_no)
                                arr_type_id.push(id);
                        }
                        calculate_total_amount_by_type("", arr_type_id, job_no);
                    }
                    $.each(item, function (index1, item1) {
                        if (index1 == item.length - 1)
                            str_btn_add = "<a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='append_item_row(this)'><span class='glyphicon glyphicon-plus'></span></a>";
                        $('#boq_table').find('tr').eq(row_index + index1).after("" +
                            "<tr class='type" + type_letter + " job" + job_no + "' id='item" + type_letter + (Number(index1) + 1) + "'>" +
                                "<td style='padding-left:50px !important;'><input type='hidden' class='product_id' value='" + item1.product_id + "' id='" + type_letter + (Number(index1) + 1) + "' />" + Number(index1 + 1) + "</td>" +
                                "<td><input type='text' class='form-control item_code' value='" + item1.product_code + "' onchange='initial_item_by_item_code(this)'/></td>" +
                                "<td><label class='item_description'>" + item1.product_name + "</label></td>" +
                                "<td></td>" +
                                "<td><label class='item_unit'>" + item1.product_unit + "</label></td>" +
                                "<td><input type='number' class='form-control item_qty' value='" + parseFloat(0).toFixed(2) + "' onchange='calculate_amount(this)'/></td>" +
                                "<td><input type='number' class='form-control item_unit_price' value='" + parseFloat(item1.unit_price).toFixed(2) + "' onchange='calculate_amount(this)'/></td>" +
                                "<td><label class='item_amount'>" + parseFloat(0).toFixed(2) + "</lable></td>" +
                                "<td><label class='item_remark'>" + remark + "</label></td>" +
                                "<td><a href='javascript:void(0)' class='btn btn-default btn-sm' onclick='remove_item_row(this)'><span class='glyphicon glyphicon-trash'></span></a></td>" +
                                "<td>" + str_btn_add + "</td>" +
                                "<td></td>" +
                                "<td></td>" +
                                "<td></td>" +
                                "<td></td>" +
                            "</tr>"
                        );
                    });
                    calculate_total_amount_by_job_category();
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
    }
}

function initial_item_by_item_code(row) {
    var ind = row.parentNode.parentNode.rowIndex;
    var item_code = $('#boq_table tr').eq(ind).find('input[type="text"].item_code').val();
    if (item_code != null || item_code != "") {
        $.ajax({
            url: '/Product/GetProductById',
            type: "get",
            dataType: "json",
            data: { p_code: item_code },
            success: function (data) {
                $.each(data, function (index, item) {
                    $('#boq_table tbody tr').eq(ind - 1).find('input[type="hidden"].product_id').val(item.product_id);
                    $('#boq_table tbody tr').eq(ind - 1).find('label.item_description').text(item.product_name);
                    $('#boq_table tbody tr').eq(ind - 1).find('label.item_unit').text(item.product_unit);
                    $('#boq_table tbody tr').eq(ind - 1).find('input[type="number"].item_unit_price').val(parseFloat(item.unit_price).toFixed(2));
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $('#boq_table tbody tr').eq(ind - 1).find('input[type="hidden"].product_id').val('');
                $('#boq_table tbody tr').eq(ind - 1).find('label.item_description').text('');
                $('#boq_table tbody tr').eq(ind - 1).find('label.item_unit').text('');
                $('#boq_table tbody tr').eq(ind - 1).find('input["number"].item_unit_price').val('');
                alert(textStatus);
            }
        });
    }
}

function calculate_amount(row) {
    var ind = row.parentNode.parentNode.rowIndex;
    var qty = $('#boq_table tr').eq(ind).find('input.item_qty').val();
    var unit_price = $('#boq_table tr').eq(ind).find('input.item_unit_price').val();
    var amount = Number(qty) * Number(unit_price);
    $('#boq_table tr').eq(ind).find('label.item_amount').text(parseFloat(amount).toFixed(2));
    calculate_total_amount_by_item(ind);
}

function calculate_total_amount_by_item(ind, arr_item_id) {
    var total_amount = 0;
    var item_class_group = $('#boq_table tr').eq(ind).attr('class').split(' ');
    var item_class_group_name = item_class_group[0];
    var item_class_group_ele = $('.' + item_class_group_name);
    if (arr_item_id == undefined || arr_item_id == null || arr_item_id == "") {
        for (var i = 1; i < item_class_group_ele.length; i++) {
            var amount = $(item_class_group_ele[i]).find('label.item_amount').text();
            total_amount = Number(total_amount) + Number(amount);
        }
    } else {
        for (var i = 0; i < arr_item_id.length; i++) {
            var amount = $('#' + arr_item_id[i]).find('label.item_amount').text();
            total_amount = Number(total_amount) + Number(amount);
        }
    }
    $(item_class_group_ele[0]).find('label.type_amount').text(parseFloat(total_amount).toFixed(2));

    var arr_type_id = new Array();
    var total_type_amount = 0;
    var type_class_num = item_class_group[1].replace(/[^\d]/g, '');
    var count_table_row = $('#boq_table tbody tr').length;
    for (var i = 1; i <= count_table_row; i++) {
        var id = $('#boq_table tr').eq(i).attr('id');
        if (id != undefined && id.substr(0, 5) == "type" + type_class_num)
            arr_type_id.push(id);
    }
    for (var i = 0; i < arr_type_id.length; i++) {
        var amount = 0;
        var ele_type = $('#boq_table tr#' + arr_type_id[i]).find('input.type_amount');
        var ele_type1 = $('#boq_table tr#' + arr_type_id[i]).find('label.type_amount');
        if (ele_type.length == 1) {
            amount = $('#boq_table tr#' + arr_type_id[i]).find('input.type_amount').val();
        }
        else if (ele_type1.length == 1) {
            amount = $('#boq_table tr#' + arr_type_id[i]).find('label.type_amount').text();
        }
        total_type_amount = Number(total_type_amount) + Number(amount);
    }
    $('#boq_table tr#' + item_class_group[1]).find('label.job_category_amount').text(parseFloat(total_type_amount).toFixed(2));
    calculate_total_amount_by_job_category();
}

function calculate_total_amount_by_type(row, arr_id, job_id) {
    var arr_type_id = new Array();
    var total_amount = 0;
    if (arr_id == undefined || arr_id == null || arr_id == "") {
        var ind = row.parentNode.parentNode.rowIndex;
        var type_class_group = $('#boq_table tr').eq(ind).attr('class').split(' ');
        var type_class_group_num = type_class_group[1].replace(/[^\d]/g, '');
        var count_table_row = $('#boq_table tbody tr').length;
        for (var i = 1; i <= count_table_row; i++) {
            var id = $('#boq_table tr').eq(i).attr('id');
            if (id != undefined && id.substr(0, 5) == "type" + type_class_group_num)
                arr_type_id.push(id);
        }
        for (var i = 0; i < arr_type_id.length; i++) {
            var amount = 0;
            var ele_type = $('#boq_table tr#' + arr_type_id[i]).find('input.type_amount');
            var ele_type1 = $('#boq_table tr#' + arr_type_id[i]).find('label.type_amount');
            if (ele_type.length == 1) {
                amount = $('#boq_table tr#' + arr_type_id[i]).find('input.type_amount').val();
            }
            else if (ele_type1.length == 1) {
                amount = $('#boq_table tr#' + arr_type_id[i]).find('label.type_amount').text();
            }
            total_amount = Number(total_amount) + Number(amount);
        }
        $('#boq_table tr#' + type_class_group[1]).find('label.job_category_amount').text(parseFloat(total_amount).toFixed(2));
    } else {
        for (var i = 0; i < arr_id.length; i++) {
            var amount = 0;
            var ele_type = $('#boq_table tr#' + arr_id[i]).find('input.type_amount');
            var ele_type1 = $('#boq_table tr#' + arr_id[i]).find('label.type_amount');
            if (ele_type.length == 1) {
                amount = $('#boq_table tr#' + arr_id[i]).find('input.type_amount').val();
            }
            else if (ele_type1.length == 1) {
                amount = $('#boq_table tr#' + arr_id[i]).find('label.type_amount').text();
            }
            total_amount = Number(total_amount) + Number(amount);
        }
        $('#boq_table tr#' + job_id).find('label.job_category_amount').text(parseFloat(total_amount).toFixed(2));
    }
    calculate_total_amount_by_job_category();
}

function calculate_total_amount_by_job_category() {
    var total_amount = 0, vat = 0, total = 0;
    var arr_job_category_id = new Array();
    var count_table_row = $('#boq_table tbody tr').length;
    for (var i = 1; i <= count_table_row; i++) {
        var id = $('#boq_table tr').eq(i).attr('id');
        if (id != undefined && id.substr(0, 3) == "job")
            arr_job_category_id.push(id);
    }
    for (var i = 0; i < arr_job_category_id.length; i++) {
        var amount = 0;
        var ele_type = $('#boq_table tr#' + arr_job_category_id[i]).find('input.job_category_amount');
        var ele_type1 = $('#boq_table tr#' + arr_job_category_id[i]).find('label.job_category_amount');
        if (ele_type.length == 1) {
            amount = $('#boq_table tr#' + arr_job_category_id[i]).find('input.job_category_amount').val();
        }
        else if (ele_type1.length == 1) {
            amount = $('#boq_table tr#' + arr_job_category_id[i]).find('label.job_category_amount').text();
        }
        total_amount = Number(total_amount) + Number(amount);
    }
    vat = (Number(total_amount) * 10) / 100;
    total = Number(total_amount) - Number(vat);
    $('#sub_total').text(parseFloat(total_amount).toFixed(2));
}

function InitialRemark(row) {
    var ind = row.parentNode.parentNode.rowIndex;
    var remark = $('#boq_table tr').eq(ind).find('select.remark').val();
    var id = $('#boq_table tr').eq(ind).attr('id');
    var count_table_row = $('#boq_table tbody tr').length;
    for (var i = 1; i <= count_table_row; i++) {
        $('#boq_table tr.' + id).eq(i).find('label.item_remark').text(remark);
        $('#boq_table tr.' + id).eq(i).find('label.type_remark').text(remark);
    }
}

function initial_job_category_code(project_short_name) {
    var count_table_row = $('#boq_table tbody tr').length;
    var arr_job_id = new Array();
    for (var i = 1; i <= count_table_row; i++) {
        var id = $('#boq_table tr').eq(i).attr('id');
        if (id != undefined && id.substr(0, 3) == "job")
            arr_job_id.push(id);
    }
    for (var i = 0; i < arr_job_id.length; i++) {
        var row_no = Number(i + 1);
        var j_category_last_split = (row_no.toString().length == 1) ? "00" + row_no : (row_no.toString().length == 2) ? "0" + row_no : row_no;
        var job_category_code = "J-" + project_short_name + "-" + j_category_last_split;
        $('#boq_table tr#' + arr_job_id[i]).find('label.job_category_code').text(job_category_code);
    }
}