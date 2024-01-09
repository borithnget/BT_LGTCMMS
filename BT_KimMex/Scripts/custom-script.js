function getFormattedDateMMDDYYYY(date) {

    var year = date.getFullYear();

    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;

    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;

    return day + '/' + month + '/' + year;
}
function convertDDMMYYYtoMMDDYYYY(date) {
    if (date == null || date == undefined || date == "")
        return date;
    var splitDate = date.split('/');
    return splitDate[1] + "/" + splitDate[0] + "/" + splitDate[2];
}
function GetURLParameter() {
    var sPageURL = window.location.href;
    var indexOfLastSlash = sPageURL.lastIndexOf("/");

    if (indexOfLastSlash > 0 && sPageURL.length - 1 != indexOfLastSlash)
        return sPageURL.substring(indexOfLastSlash + 1);
    else
        return 0;
}
