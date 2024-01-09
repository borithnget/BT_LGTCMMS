var append_po_row;
var remove_po_row;
var get_item_data;

append_po_row = function (row,isPur) {
    var ind = row.parentNode.parentNode.rowIndex;
    var item_value = $('#po_table tr').eq(ind).find('input.item_id').val();
    if (!item_value) {
        return;
    }
    var previous_number = $('#po_table tr').eq(ind).find('td:first-child').text();
    var current_number = Number(previous_number)+1;
    var previous_id = $('#po_table tr').eq(ind).attr('id');
    var id_number = previous_id.replace(/[^\d]/g, '');
    var current_id = previous_id.replace(new RegExp('[0-9]'), '') + (Number(id_number) + 1);
    if (isPur) {
        $('#po_table tbody').append("" +
            "<tr id='" + current_id + "'>" +
                "<td>" + current_number + "</td>" +
                "<td><input type='hidden' class='item_id'/><input type='text' class='form-control item_code' id='item_code" + (Number(id_number) + 1) + "' onfocus='get_item_data(this)'/></td>" +
                "<td><label class='item_name'></label></td>" +
                "<td><label class='item_unit'></label></td>" +
                "<td><label class='approved_qty'></label></td>" +
                "<td><input type='number' class='form-control po_item_qty' value='0'/></td>" +
                "<td><a href='javascript:void(0)' class='btn btn-default' onclick='remove_po_row(this,true)'><span class='glyphicon glyphicon-remove-sign'></span></a></td>" +
                "<td><a href='javascript:void(0)' class='btn btn-default' onclick='append_po_row(this,true)'><span class='glyphicon glyphicon-plus-sign'></span></a></td>" +
            "</tr>"
        );
        $('#po_table tr').eq(ind).find('td:nth-child(8)').html('');
    }
    else {
        $('#po_table tbody').append("" +
            "<tr id='" + current_id + "'>" +
                "<td>" + current_number + "</td>" +
                "<td><input type='hidden' class='item_id'/><input type='text' class='form-control item_code' id='item_code" + (Number(id_number) + 1) + "' onfocus='get_item_data(this)'/></td>" +
                "<td><label class='item_name'></label></td>" +
                "<td><label class='item_unit'></label></td>" +
                "<td><input type='number' class='form-control po_item_qty' value='0'/></td>" +
                                            "<td><select class='form-control sup1_name'></select></td>" +
                                            "<td><input type='number' class='form-control sup1_unit_price' value='0.00'/></td>" +
                                            "<td><select class='form-control sup2_name'></select></td>" +
                                            "<td><input type='number' class='form-control sup2_unit_price' value='0.00'/></td>" +
                                            "<td><select class='form-control sup3_name'></select></td>" +
                                            "<td><input type='number' class='form-control sup3_unit_price' value='0.00'/></td>" +
                "<td><a href='javascript:void(0)' class='btn btn-default' onclick='remove_po_row(this,false)'><span class='glyphicon glyphicon-remove-sign'></span></a></td>" +
                "<td><a href='javascript:void(0)' class='btn btn-default' onclick='append_po_row(this,false)'><span class='glyphicon glyphicon-plus-sign'></span></a></td>" +
            "</tr>"
        );
        $('#po_table tr').eq(ind).find('td:nth-child(13)').html('');
        InitialSupplierDropdownlist(true);
    }
}

remove_po_row = function (row,isPur) {
    var ind = row.parentNode.parentNode.rowIndex;
    var remove_id = $('#po_table tr').eq(ind).attr('id');
    var is_last_child;
    //if(isPur)
    //    is_last_child = $('#po_table tr').eq(ind).find('td:nth-child(7) a').length == 0 ? false : true;
    //else
        is_last_child = $('#po_table tr').eq(ind).find('td:nth-child(13) a').length == 0 ? false : true;
    var arr_id = new Array();
    var count_item = $('#po_table tr').length;
    for (var i = 1; i <= count_item; i++) {
        var id = $('#po_table tr').eq(i).attr('id');
        if (id)
            arr_id.push(id);
    }
    if (arr_id.length > 1) {
        document.getElementById('po_table').deleteRow(ind);
        if (is_last_child)
            //if(isPur)
            //    $('#po_table tr').eq(ind - 1).find('td:nth-child(7)').html("<a href='javascript:void(0)' class='btn btn-default' onclick='append_po_row(this,true)'><span class='glyphicon glyphicon-plus-sign'></span></a>");
            //else
                $('#po_table tr').eq(ind - 1).find('td:nth-child(13)').html("<a href='javascript:void(0)' class='btn btn-default' onclick='append_po_row(this,false)'><span class='glyphicon glyphicon-plus-sign'></span></a>");
        else {
            var deleted_index = arr_id.indexOf(remove_id);
            arr_id.splice(deleted_index, 1);
            for (var i = 0; i < arr_id.length; i++) {
                $('tr#' + arr_id[i]).find('td:first-child').html(Number(i + 1));
            }
        }
    }
}

get_item_data = function (row) {

    var ind = row.parentNode.parentNode.rowIndex;
    var row_id = $('#po_table tr').eq(ind).find('input.item_code').attr('id');
    var ir_id = $('#item_request_id').val();
    var item_code;
    var item_id;

    autocompleted(row_id,ir_id,ind);
    
    function autocompleted(row_id,ir_id,ind) {
        $("#"+row_id).autocomplete({
            source: '/PurchaseOrder/GetItemAutoSuggestionByCodes',
            select: function (event, ui) {
                AutoCompleteSelectHandler(event, ui,ir_id,ind);
            }
        });
    }

    function AutoCompleteSelectHandler(event, ui,ir_id,ind) {
        var selectedObj = ui.item;
        var item = (selectedObj.value).split(' ');
        item_id = selectedObj.id;
        item_code = item[0];
        GetItemDataRow(ir_id,item_id,item_code,ind)
    }
    function GetItemDataRow(ir_id, item_id,item_code,ind) {
        if (ir_id && item_id) {
            $.ajax({
                url: '/PurchaseOrder/GetItemDataByIRID',
                type: "get",
                dataType: "json",
                data: { ir_id: ir_id, item_id:item_id },
                success: function (da) {
                    if (da.result == "success") {
                        if (da.data) {
                            var item = da.data;
                            $('#po_table tr').eq(ind).find('input.item_id').val(item.ir_item_id);
                            $('#po_table tr').eq(ind).find('input.item_code').val(item.product_code);
                            $('#po_table tr').eq(ind).find('label.item_name').text(item.product_name);
                            $('#po_table tr').eq(ind).find('label.item_unit').text(item.product_unit);
                        }
                        else {
                            $('#po_table tr').eq(ind).find('input.item_id').val('');
                            $('#po_table tr').eq(ind).find('input.item_code').val(item_code);
                            $('#po_table tr').eq(ind).find('label.item_name').text(' ');
                            $('#po_table tr').eq(ind).find('label.item_unit').text(' ');
                        }   
                    } else {
                        $('#po_table tr').eq(ind).find('input.item_id').val('');
                        $('#po_table tr').eq(ind).find('input.item_code').val(item_code);
                        $('#po_table tr').eq(ind).find('label.item_name').text(' ');
                        $('#po_table tr').eq(ind).find('label.item_unit').text(' ');
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $('#po_table tr').eq(ind).find('input.item_id').val('');
                    $('#po_table tr').eq(ind).find('input.item_code').val(item_code);
                    $('#po_table tr').eq(ind).find('label.item_name').text(' ');
                    $('#po_table tr').eq(ind).find('label.item_unit').text(' ');
                }
            });
        }
    }
}