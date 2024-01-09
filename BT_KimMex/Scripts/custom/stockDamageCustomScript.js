function initialTypeRow(ind, rowLetter) {
    $('#stockDamageTable tr').eq(ind).after("" +
            "<tr id='type" + rowLetter + "' class='" + rowLetter + "'>" +
                "<td>" + rowLetter + "</td>" +
                "<td><input type='hidden' class='type-id'/></td>" +
                "<td><input type='text' class='form-control type-code' id='" + rowLetter + "' onfocus='getItemDateRow(this)' placeholder='Item Type Code'/></td>" +
                "<td><label class='type-description'></label></td>" +
                "<td></td>" +
                "<td></td>" +
                "<td></td>" +
                "<td></td>" +
                "<td></td>" +
                "<td></td>" +
                "<td><a href='javascript:void(0)' onclick='removeTypeRow(this)'><i class='fa fa-minus-circle' style='font-size:30px;color:black'></i></a></td>" +
                "<td><a href='javascript:void(0)' onclick='appendTypeRow(this)'><i class='fa fa-plus-circle' style='font-size:30px;color:black'></i></a></td>" +
            "</tr>"
        );
}

function initialItemRow(ind, rowLetter, rowNumber) {
    $('#stockDamageTable tr').eq(ind).after("" +
                                "<tr class='" + rowLetter + "' id='item" + rowLetter + rowNumber + "'>" +
                                    "<td>" + rowNumber + "</td>" +
                                    "<td><select class='form-control warehouse' onchange='InitialItemInventory(this)'><option value>Select Warehouse</option></select></td>" +
                                    "<td><input type='hidden' class='item-id'/><input type='text' class='form-control item-code' placeholder='Item Code'/></td>" +
                                    "<td><label class='item-name'></label></td>" +
                                    "<td><label class='item-unit'/></td>" +
                                    "<td><label class='stock-balance'>" + parseFloat(0).toFixed(2) + "</label></td>" +
                                    "<td><input type='number' class='form-control damage-qty' value='" + parseFloat(0).toFixed(2) + "' onchange='checkDamageQty(this)'/></td>" +
                                    "<td><textarea class='form-control remark'></textarea></td>" +
                                    "<td><a href='javascript:void(0)' onclick='removeItemRow(this)'><i class='fa fa-minus-square' style='font-size:30px;color:black'></i></a></td>" +
                                    "<td><a href='javascript:void(0)' onclick='appendItemRow(this)'><i class='fa fa-plus-square' style='font-size:30px;color:black'></i></a></td>" +
                                "</tr>"
        );
    InitialItemWarehouse(ind);
}

function appendTypeRow(row) {
    var ind = row.parentNode.parentNode.rowIndex;
    var typeClass = $('#stockDamageTable tr').eq(ind).attr('class');
    var rowLetter = $('#stockDamageTable tr').eq(ind).find('td:first-child').text();
    var countTypeClass = $('.' + typeClass).length;
    var typeLetter = String.fromCharCode(Number(rowLetter.charCodeAt(0) + 1));
    if (countTypeClass == 1)
        return;
    initialTypeRow(Number(ind + countTypeClass - 1), typeLetter);
    $('#stockDamageTable tr').eq(ind).find('td:last-child').html('');
}

function removeTypeRow(row) {
    var ind = row.parentNode.parentNode.rowIndex;
    var arrTypeId = [];
    var isLastChild = $('#stockDamageTable tr').eq(ind).find("td:nth-child(12) a").length == 0 ? false : true;
    var typeClass = $('#stockDamageTable tr').eq(ind).attr('class');
    var typeId = $('#stockDamageTable tr').eq(ind).attr('id');
    var countTypeClass = $('.' + typeClass).length;
    var count_table_row = $('#stockDamageTable tr').length;
    for (var i = 1; i <= count_table_row; i++) {
        var id = $('#stockDamageTable tr').eq(i).attr('id');
        if (id != undefined && id.substr(0, 4) == "type")
            arrTypeId.push(id);
    }
    if (arrTypeId.length == 1) {
        return;
    }
    for (var i = 0; i < countTypeClass; i++) {
        document.getElementById('stockDamageTable').deleteRow(ind);
    }
    if (isLastChild) {
        $('#stockDamageTable tr#' + arrTypeId[arrTypeId.length - 2]).find('td:last-child').html("<a href='javascript:void(0)' onclick='appendTypeRow(this)'><i class='fa fa-plus-circle' style='font-size:30px;color:black'></i></a>");
    } else {
        var deleted_index = arrTypeId.indexOf(typeId);
        arrTypeId.splice(deleted_index, 1);
        for (var i = 0; i < arrTypeId.length; i++) {
            var letter = String.fromCharCode(Number(65 + i));
            $('tr#' + arrTypeId[i] + ' td:nth-child(1)').html(letter);
        }
    }
}

function appendItemRow(row) {
    var ind = row.parentNode.parentNode.rowIndex;
    var typeClass = $('#stockDamageTable tr').eq(ind).attr('class');
    var countTypeClass = $('.' + typeClass).length;
    var rowNumber = $('#stockDamageTable tr').eq(ind).find('td:first-child').text();
    var warehouseId = $('#stockDamageTable tr').eq(ind).find('select.warehouse').val();
    var itemId = $('#stockDamageTable tr').eq(ind).find('input.item-id').val();
    var damageQty = $('#stockDamageTable tr').eq(ind).find('input.damage-qty').val();
    if (!warehouseId || !itemId || Number(damageQty) <= 0) {
        return;
    }
    initialItemRow(ind, typeClass, Number(rowNumber) + 1);
    $('#stockDamageTable tr').eq(ind).find('td:nth-child(10)').html('');
    $('#stockDamageTable tr').eq(countTypeClass - (countTypeClass - 1)).find('td:nth-child(11)').attr('rowspan', Number(countTypeClass + 1));
    $('#stockDamageTable tr').eq(countTypeClass - (countTypeClass - 1)).find('td:nth-child(12)').attr('rowspan', Number(countTypeClass + 1));
}

function removeItemRow(row) {
    var arrItemId = [];
    var ind = row.parentNode.parentNode.rowIndex;
    var isLastChild = $('#stockDamageTable tr').eq(ind).find("td:nth-child(10) a").length == 0 ? false : true;
    var itemClass = $('#stockDamageTable tr').eq(ind).attr('class');
    var itemId = $('#stockDamageTable tr').eq(ind).attr('id');
    var count_table_row = $('#stockDamageTable tr').length;
    for (var i = 1; i <= count_table_row; i++) {
        var id = $('#stockDamageTable tr').eq(i).attr('id');
        if (id != undefined && id.substr(0, 5) == "item" + itemClass)
            arrItemId.push(id);
    }
    if (arrItemId.length == 1) {
        return;
    }
    document.getElementById('stockDamageTable').deleteRow(ind);
    if (isLastChild)
        $('#stockDamageTable tr#' + arrItemId[arrItemId.length - 2]).find('td:nth-child(10)').html("<a href='javascript:void(0)' onclick='appendItemRow(this)'><i class='fa fa-plus-square' style='font-size:30px;color:black'></i></a>");
    else {
        var deleted_index = arrItemId.indexOf(itemId);
        arrItemId.splice(deleted_index, 1);
        for (var i = 0; i < arrItemId.length; i++) {
            $('tr#' + arrItemId[i] + ' td:nth-child(1)').html(Number(i) + 1);
        }
    }
}

function getItemDateRow(row) {
    var ind = row.parentNode.parentNode.rowIndex;
    var rowId = $('#stockDamageTable tr').eq(ind).find('input[type="text"].type-code').attr('id');
    var typeClass = $('#stockDamageTable tr').eq(ind).attr('class');
    var category_id;
    autocompleted(rowId, typeClass);

    function autocompleted(rowId, typeClass) {
        $("#" + rowId).autocomplete({
            source: '/ProductCategory/GetProductCategoryCodes',
            select: function (event, ui) {
                AutoCompleteSelectHandler(event, ui, ind, typeClass);
            }
        });
    }

    function AutoCompleteSelectHandler(event, ui, ind, typeClass) {
        var selectedObj = ui.item;
        var item = (selectedObj.value).split(' ');
        category_id = selectedObj.id;
        GetItemTypeDataRow(ind, category_id, typeClass)
    }

    function GetItemTypeDataRow(ind, item_type_id, typeClass) {
        if (item_type_id != "") {
            $.ajax({
                url: '/ProductCategory/GetProductCategoryInfo',
                type: "get",
                dataType: "json",
                async: false,
                data: { category_id: item_type_id },
                success: function (data) {
                    $.each(data, function (index, item) {
                        $('#stockDamageTable tbody tr').eq(ind).find('input[type="hidden"].type-id').val(item.p_category_id);
                        $('#stockDamageTable tbody tr').eq(ind).find('input[type="text"].type-code').val(item.p_category_code);
                        $('#stockDamageTable tbody tr').eq(ind).find('label.type-description').text(item.p_category_name);
                        category_id = item.p_category_id;
                        GetItemDataRow(ind, category_id, typeClass)
                    });
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $('#stockDamageTable tbody tr').eq(ind).find('input[type="hidden"].type-id').val('');
                    $('#stockDamageTable tbody tr').eq(ind).find('label.type-description').text('');
                    alert(textStatus);
                }
            });
        }
    }

    function GetItemDataRow(rowIndex, typeId, typeClass) {
        if (typeId != "") {
            var itemClass = $('.' + typeClass);
            var strBtnAdd = "";
            var countRow = 0;
            if (itemClass.length > 0) {
                for (var i = 1; i < itemClass.length; i++) {
                    var idx = itemClass[i].rowIndex;
                    document.getElementById('stockDamageTable').deleteRow(idx);
                }
            }
            $.ajax({
                url: '/Product/GetProductDataRow',
                type: "get",
                dataType: "json",
                async: false,
                data: { category_id: typeId },
                success: function (data) {
                    $.each(data, function (index, item) {
                        countRow = item.length;
                        $.each(item, function (index1, item1) {
                            if (index1 == item.length - 1)
                                strBtnAdd = "<a href='javascript:void(0)' onclick='appendItemRow(this)'><i class='fa fa-plus-square' style='font-size:30px;color:black'></i></a>";
                            $('#stockDamageTable').find('tr').eq(rowIndex + index1).after("" +
                                "<tr class='" + typeClass + "' id='item" + typeClass + (Number(index1) + 1) + "'>" +
                                    "<td>" + (Number(index1) + 1) + "</td>" +
                                    "<td><select class='form-control warehouse' onchange='InitialItemInventory(this)'><option value>Select Warehouse</option></select></td>" +
                                    "<td><input type='hidden' class='item-id' value='" + item1.product_id + "'/><input type='text' class='form-control item-code' value='" + item1.product_code + "'/></td>" +
                                    "<td><label class='item-name'>" + item1.product_name + "</label></td>" +
                                    "<td><label class='item-unit'/>" + item1.product_unit + "</td>" +
                                    "<td><label class='stock-balance'>" + parseFloat(0).toFixed(2) + "</label></td>" +
                                    "<td><input type='number' class='form-control damage-qty' value='" + parseFloat(0).toFixed(2) + "' onchange='checkDamageQty(this)'/></td>" +
                                    "<td><textarea class='form-control remark'></textarea></td>" +
                                    "<td><a href='javascript:void(0)' onclick='removeItemRow(this)'><i class='fa fa-minus-square' style='font-size:30px;color:black'></i></a></td>" +
                                    "<td>" + strBtnAdd + "</td>" +
                                "</tr>"
                            );
                            InitialItemWarehouse(rowIndex + index1);
                        });


                    });
                    $('#stockDamageTable tr').eq(rowIndex).find('td:nth-child(11)').attr('rowspan', Number(countRow + 1));
                    $('#stockDamageTable tr').eq(rowIndex).find('td:last-child').attr('rowspan', Number(countRow + 1));
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus);
                }
            });
        }
    }
}

function InitialItemWarehouse(ind) {
    $.ajax({
        url: '/Warehouse/GetWarehouseDropdownList',
        type: "get",
        dataType: "json",
        async: false,
        success: function (da) {
            if (da.result == "success") {
                $.each(da.data, function (index, item) {
                    $('#stockDamageTable tr').eq(ind + 1).find('select.warehouse').append("<option value='" + item.warehouse_id + "'>" + item.warehouse_name + "</option>");
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

        }
    });
}

function InitialItemInventory(row) {
    var ind = row.parentNode.parentNode.rowIndex;
    var itemId = $('#stockDamageTable tr').eq(ind).find('input[type="hidden"].item-id').val();
    var warehouseId = $('#stockDamageTable tr').eq(ind).find('select.warehouse').val();
    if (!itemId || !warehouseId) {
        return;
    }
    $.ajax({
        url: '/StockDamage/GetInventoryItem',
        type: "get",
        dataType: "json",
        async: false,
        data: { itemId: itemId, warehouseId: warehouseId },
        success: function (da) {
            if (da.result == "success") {
                if (da.data) {
                    $('#stockDamageTable tr').eq(ind).find('label.stock-balance').text(parseFloat(da.data.total_quantity).toFixed(2));
                } else {
                    $('#stockDamageTable tr').eq(ind).find('label.stock-balance').text(parseFloat(0).toFixed(2));
                }
                var damageQty = $('#stockDamageTable tr').eq(ind).find('input.damage-qty').val();
                if (Number(da.data.total_quantity) < Number(damageQty)) {
                    alert("Damage item quantity must be smaller than Stock balance.");
                    return;
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

        }
    });
}
function checkDamageQty(row) {
    var ind = row.parentNode.parentNode.rowIndex;
    var stockBalance = $('#stockDamageTable tr').eq(ind).find('label.stock-balance').text();
    var damageQty = $('#stockDamageTable tr').eq(ind).find('input.damage-qty').val();
    if (Number(stockBalance) < Number(damageQty)) {
        alert("Damage item quantity must be smaller than Stock balance.");
        return;
    }
}

function saveStockDamage() {
    var inventories = [];
    var item_element = {};
    var warehouses = $('.warehouse');
    var items = $('.item-id');
    var stockBalances = $('.stock-balance');
    var damageQty = $('.damage-qty');
    var remark = $('.remark');
    var countInvalid = 0;
    for (var i = 0; i < items.length; i++) {
        if (warehouses[i].value && items[i] && stockBalances[i].innerHTML && damageQty[i].value) {
            item_element = {};
            item_element.warehouse_id = warehouses[i].value;
            item_element.product_id = items[i].value;
            item_element.total_quantity = stockBalances[i].innerHTML;
            item_element.out_quantity = damageQty[i].value;
            item_element.remark = remark[i].value;
            inventories.push(item_element);
        }
        else {
            countInvalid++;
        }
    }
    if (countInvalid > 0) {
        alert("Please select warehouse and fill in damage qty");
        return;
    }

    var model = {
        stock_damage_number: $('#stock_damage_number').text(),
        inventories: inventories,
    };

    $.ajax({
        url: "/StockDamage/Create",
        type: "post",
        dataType: "json",
        async: false,
        data: { model: model },
        success: function (da) {
            if (da.result == "success") {
                $.notify('Your data has been saved!', { className: 'success' });
                window.location.href = '/StockDamage/Index';
            } else if (da.result == "error") {
                $.notify(da.message, { className: 'error' });
            }
        },
        error: function (err) {
            $.notify('Your data is error while saving!', { className: 'error' });
        }
    });
}