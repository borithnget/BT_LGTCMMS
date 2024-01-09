function InitialStockTransferDropdownList(id) {
    $.ajax({
        url: "/ItemReceive/GetStockTransferNumberDropdownlist",
        type: "get",
        dataType: "json",
        success: function (da) {
            if (da.data) {
                $.each(da.data, function (index, item) {
                    $('#ref_id').append("" +
                       "<option value='" + item.stock_transfer_id + "'>" + item.stock_transfer_no + "</option>"
                        );
                });
                if (id != undefined)
                    $('#ref_id').val(id);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

function InitialPurchaseOrderDropdownList(id) {
    $.ajax({
        url: '/ItemReceive/GetPurchaseOrderNumberDropdownlist',
        type: "get",
        dataType: "json",
        success: function (da) {
            if (da.data) {
                $.each(da.data, function (index, item) {
                    $('#ref_id').append("" +
                       "<option value='" + item.purchase_order_id + "'>" + item.purchase_oder_number + "</option>"
                        );
                });
                if (id != undefined)
                    $('#ref_id').val(id);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

function initialReceivedItem(isST, stID) {
    var receivedType = $('input[type=radio][name=receiveType]:checked').val();
    $('#itemReceiveTable tbody').empty();
    if (receivedType == "Stock Transfer" && isST && stID) {
        $.ajax({
            url: '/ItemReceive/GetStockTransferItem',
            type: "get",
            dataType: "json",
            data: { id: stID },
            success: function (da) {
                if (da.result == "success") {
                    $.each(da.data, function (index, item) {
                        $('#itemReceiveTable').append("" +
                                "<tr>" +
                                    "<td>" + Number(index + 1) + "</td>" +
                                    "<td><input type='hidden' class='item_id' value='" + item.product_id + "'/>" + item.itemCode + "</td>" +
                                    "<td>" + item.itemName + "</td>" +
                                    "<td>" + item.itemUnit + "</td>" +
                                    "<td><label class='request_qty'>" + parseFloat(item.out_quantity).toFixed(2) + "</label></td>" +
                                    "<td><input type='number' class='form-control received_qty' value='0.00' onchange='checkQty(this)' /> </td>" +
                                    "<td><select class='form-control warehouse'><option value>Select Warehouse</option></select></td>" +
                                "</tr>"
                            );
                        InitialWarehouseDropdownList(isST, stID, Number(index + 1));
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });
    } else if (receivedType == "Purchase Order" && isST == false && stID) {
        $.ajax({
            url: '/ItemReceive/GetPurchaseOrderItem',
            type: "get",
            dataType: "json",
            data: { id: stID },
            success: function (da) {
                if (da.result == "success") {
                    $.each(da.data, function (index, item) {
                        $('#itemReceiveTable').append("" +
                                "<tr>" +
                                    "<td>" + Number(index + 1) + "</td>" +
                                    "<td><input type='hidden' class='item_id' value='"+item.item_id+"'/>" + item.product_code + "</td>" +
                                    "<td>" + item.product_name + "</td>" +
                                    "<td>" + item.product_unit + "</td>" +
                                    "<td><label class='request_qty'>" + parseFloat(item.quantity).toFixed(2) + "</label></td>" +
                                    "<td><input type='number' class='form-control received_qty' value='0.00' onchange='checkQty(this)' /> </td>" +
                                    "<td><select class='form-control warehouse'><option value>Select Warehouse</option></select></td>" +
                                "</tr>"
                            );
                        InitialWarehouseDropdownList(isST, stID, Number(index + 1));
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });
    }
}

function InitialWarehouseDropdownList(isST, ID, ind) {
    if (isST) {
        $.ajax({
            url: '/ItemReceive/GetWarehouseByStockTransfer',
            type: "get",
            dataType: "json",
            data: { id: ID },
            success: function (da) {
                if (da.result == "success") {
                    $.each(da.data, function (index, item) {
                        $('#itemReceiveTable tr').eq(ind).find('select.warehouse').append("<option value='" + item.warehouse_id + "'>" + item.warehouse_name + "</option>");
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });
    } else {
        $.ajax({
            url: '/ItemReceive/GetWarehouseByPurchaseOrder',
            type: "get",
            dataType: "json",
            data: { id: ID },
            success: function (da) {
                if (da.result == "success") {
                    $.each(da.data, function (index, item) {
                        $('#itemReceiveTable tr').eq(ind).find('select.warehouse').append("<option value='" + item.warehouse_id + "'>" + item.warehouse_name + "</option>");
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });
    }
}

function checkQty(row) {
    var ind = row.parentNode.parentNode.rowIndex;
    var requestQty = $('#itemReceiveTable tr').eq(ind).find('label.request_qty').text();
    var receiveQty = $('#itemReceiveTable tr').eq(ind).find('input.received_qty').val();
    if (receiveQty && (Number(receiveQty) > Number(requestQty))) {
        $.notify('Receive Qty must be smaller than Request Qty.', { className: 'error' });
        $('#itemReceiveTable tr').eq(ind).find('input.received_qty').focus();
        return;
    }
}