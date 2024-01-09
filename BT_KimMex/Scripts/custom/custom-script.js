function GetURLParameter() {
    var sPageURL = window.location.href;
    var indexOfLastSlash = sPageURL.lastIndexOf("/");

    if (indexOfLastSlash > 0 && sPageURL.length - 1 != indexOfLastSlash)
        return sPageURL.substring(indexOfLastSlash + 1);
    else
        return 0;
}

class ShowDialogFunction {

    static ShowDialogWithInput(id,ptype,status) {
        swal.fire({
            title: 'Please input your comment:',
            input: 'text',
            inputAttributes: {
                autocapitalize: 'off'
            },
            showCancelButton: true,
            confirmButtonText: 'Submit',
            showLoaderOnConfirm: true,
            preConfirm: (value) => {
                return { comment: value };
            },
            allowOutsideClick: () => !swal.isLoading()
        }).then((result) => {
            if (result.isConfirmed) {
                //console.log(result.value.comment);
                // A dialog has been submitted
                if (ptype == "quote" && status=="request_cancel") {
                    Response.PurchaseOrderReqeustCancel(id, result.value.comment);
                } else if (ptype == "quote" && status == "reject") {
                    Response.PurchaseOrderRejectResubmit(id, result.value.comment);
                }
            }

        })
    }

    static ShowValidateMessage(err) {
        Swal.showValidationMessage(`Request failed: ${err}`);
    }
    static ShowSuccessMessage(messge) {
        swal.fire({ title: messge });
    }
}

class Response {
    static PurchaseOrderReqeustCancel(id, comment) {
            $.ajax({
                url: "/PurchaseOrder/RequestCancel",
                data: {
                    'id': id,
                    'comment': comment,
                },
                type: 'GET',
                success: function (da) {
                    console.log(da);
                    swal.fire({ title: 'Sucess' });
                    window.location.href = '/PurchaseOrder/MyRequest';
                },
                error: function (err) {
                    Swal.showValidationMessage(`Request failed: ${err}`)
                }
            });
    }

    static PurchaseOrderRejectResubmit(id, comment) {
        $.ajax({
            url: "/PurchaseOrder/RejectResubmit",
            data: {
                'id': id,
                'comment': comment,
            },
            type: 'GET',
            success: function (da) {
                console.log(da);
                swal.fire({ title: 'Sucess' });
                window.location.href = '/PurchaseOrder/MyApproval';
            },
            error: function (err) {
                Swal.showValidationMessage(`Request failed: ${err}`)
            }
        });
    }
}