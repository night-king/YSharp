
var selectedIdArray = [];
var selectedId;
var selectedIds;


function openPage(a, url, isMustSelected, isOpenNewPage) {
    if (isMustSelected === 1) {
        var tr = $(a).closest('tr');
        var itemid = $(tr).attr("itemid");
        if (!itemid) {
            var tag = $(tr).find("td").eq(0).children()[0];
            itemid = $(tag).attr("itemid");
        }
        url += url.indexOf("?") > 0 ? "&id=" + itemid : "?id=" + itemid;
    }
    if (isOpenNewPage === 1) {
        window.open(url);
    } else {
        location.href = url;
    }
}

function openDialog(a, title, url, w, h, isMustSelected) {
    if (isMustSelected === 1) {
        var tr = $(a).closest('tr');
        var itemid = $(tr).attr("itemid");
        if (!itemid) {
            var tag = $(tr).find("td").eq(0).children()[0];
            itemid = $(tag).attr("itemid");
        }
        url += (url.indexOf("?") > 0 ? "&id=" + itemid : "?id=" + itemid) + "&ids=" + selectedIds;
    }
    $('#modal-window .ibox-title h5').empty().append(title);
    $('#modal-window .modal-dialog').css("width", w + "px");
    $('#modal-window .ibox-content').css("height", h + "px");
    $('#modal-window').modal('show');
    $('#modal-window .ibox-content').empty().append(
        '<iframe id="iframepage" src="' + url + '"  width="100%"  height="100%" style="background-color:transparent" frameborder="0" scrolling="auto" marginheight="0" marginwidth="0"> </iframe>'
    );
}

function closeDialog() {
    $(this.window.parent.document).find("#modal-window #close-btn").click();
}

$(function () {
    $('.single-select-table tr').click(function () {
        var tr = $(this);
        var itemid = $(this).attr("itemid");
        if (!itemid) {
            var tag = $(this).find("td").eq(0).children()[0];
            itemid = $(tag).attr("itemid");
        }
        $('.single-select-table tbody tr').removeClass("active").attr("state", 0);
        //$('.single-select-table tbody tr').find('.button[requireid=true]').addClass("disabled").attr("disabled", "disabled");
        $(tr).addClass("active").attr("state", 1);
        selectedId = itemid;
        //$('.page-toolbars .button[requireid=true]').removeClass("disabled").removeAttr("disabled");
        //$(tr).find('.button').removeClass("disabled").removeAttr("disabled");
        console.log(selectedId);
    });

    $(".multiple-select-table #CheckAll").click(function (e) {
        var $input = $(this);
        var ischecked = $(this).is(":checked");
        if (ischecked) {
            $('.multiple-select-table tbody tr td input').prop("checked", true);
            $(".multiple-select-table tr").addClass("active");
            $(".multiple-select-table tr").attr("state", 1);
            $(".multiple-select-table tr").find('.button').removeClass("disabled").removeAttr("disabled");
            selectedIds = "";
            $.each($(".multiple-select-table tbody tr input"), function (i, item) {
                var itemid = $(item).attr("itemId");
                if (itemid) {
                    selectedIdArray.push(itemid);
                }
            });
            jQuery.unique(selectedIdArray);
            for (var i = 0; i < selectedIdArray.length; i++) {
                selectedIds += selectedIdArray[i] + ",";
            }
            if (selectedIdArray.length > 0) {
                selectedId = selectedIdArray[0];
                $('#deleteSelected').removeAttr("disabled");
                $('.page-toolbars .button[requireid=true]').removeClass("disabled").removeAttr("disabled");
            } else {
                $('#deleteSelected').attr("disabled", "disabled");
                $('.page-toolbars .button[requireid=true]').addClass("disabled").attr("disabled", "disabled");
            }
            console.log("selectedId:" + selectedIds);
        }
        else {
            $('.multiple-select-table  tbody  tr  td  input').prop("checked", false);
            $(".multiple-select-table tr").removeClass("active").removeAttr("state", 0);
            $(".multiple-select-table tr").find('.button').addClass("disabled").attr("disabled", "disabled");
            selectedIdArray = [];
            selectedIds = "";
            console.log("selectedId:" + selectedIds);
            if (selectedIdArray.length > 0) {
                selectedId = selectedIdArray[0];
                $('#deleteSelected').removeAttr("disabled");
                $('.page-toolbars .button[requireid=true]').removeClass("disabled").removeAttr("disabled");
            } else {
                $('#deleteSelected').attr("disabled", "disabled");
                $('.page-toolbars .button[requireid=true]').addClass("disabled").attr("disabled", "disabled");
            }
        }
    });

    $('.multiple-select-table tbody tr input[type=checkbox]').click(function (e) {
        if ($(e.target)[0].tagName.toLowerCase() != "input") {
            return;
        }
        var $input = $(this);
        var itemid = $input.attr("itemid");
        var ischecked = $input.is(":checked");
        if (ischecked) {
            $(".multiple-select-table tr input[itemid=" + itemid + "]").parent().parent().addClass("active").attr("state", 1);
            $(".multiple-select-table tr input[itemid=" + itemid + "]").parent().parent().find('.button[requireid=true]').removeClass("disabled").removeAttr("disabled");
            selectedIdArray.push(itemid);
            selectedIds = "";
            jQuery.unique(selectedIdArray);
            for (var i = 0; i < selectedIdArray.length; i++) {
                selectedIds += selectedIdArray[i] + ",";
            }
            console.log("selectedId:" + selectedIds);
            if (selectedIdArray.length > 0) {
                selectedId = itemid;
                $('#deleteSelected').removeAttr("disabled");
                $('.page-toolbars .button[requireid=true]').removeClass("disabled").removeAttr("disabled");
            } else {
                $('#deleteSelected').attr("disabled", "disabled");
                $('.page-toolbars .button[requireid=true]').addClass("disabled").attr("disabled", "disabled");
            }
        }
        else {
            $(".multiple-select-table tr input[itemid=" + itemid + "]").parent().parent().removeClass("active").attr("state", 0);
            $(".multiple-select-table tr input[itemid=" + itemid + "]").parent().parent().find('.button[requireid=true]').addClass("disabled").attr("disabled", "disabled");
            for (var i = 0; i < selectedIdArray.length; i++) {
                if (selectedIdArray[i] == itemid) {
                    selectedIdArray.splice(i, 1);
                    break;
                }
            }
            selectedIds = "";
            for (var i = 0; i < selectedIdArray.length; i++) {
                selectedIds += selectedIdArray[i] + ",";
            }
            if (selectedIdArray.length > 0) {
                selectedId = selectedIdArray[0];
                $('#deleteSelected').removeAttr("disabled");
                $('.page-toolbars .button[requireid=true]').removeClass("disabled").removeAttr("disabled");
            } else {
                $('#deleteSelected').attr("disabled", "disabled");
                $('.page-toolbars .button[requireid=true]').addClass("disabled").attr("disabled", "disabled");
            }
            console.log("selectedId:" + selectedIds);
        }
    });

    $('.multiple-select-table tbody tr').click(function (e) {
        if ($(e.target)[0].tagName.toLowerCase() != "td") {
            return;
        }
        var $input = $(this).find('input').eq(0);
        var ischecked = $input.is(":checked");
        var itemid = $input.attr("itemid");
        if (!ischecked) {
            $input.prop("checked", true);
            $(".multiple-select-table tr input[itemid=" + itemid + "]").parent().parent().addClass("active").attr("state", 1);
            $(".multiple-select-table tr input[itemid=" + itemid + "]").parent().parent().find('.button[requireid=true]').removeClass("disabled").removeAttr("disabled");
            selectedIdArray.push(itemid);
            selectedIds = "";
            jQuery.unique(selectedIdArray);
            for (var i = 0; i < selectedIdArray.length; i++) {
                selectedIds += selectedIdArray[i] + ",";
            }
            console.log("selectedId:" + selectedIds);
            if (selectedIdArray.length > 0) {
                selectedId = itemid;
                $('#deleteSelected').removeAttr("disabled");
                $('.page-toolbars .button[requireid=true]').removeClass("disabled").removeAttr("disabled");
            } else {
                $('#deleteSelected').attr("disabled", "disabled");
                $('.page-toolbars .button[requireid=true]').addClass("disabled").attr("disabled", "disabled");
            }
        }
        else {
            $input.prop("checked", false);
            $(".multiple-select-table tr input[itemid=" + itemid + "]").parent().parent().removeClass("active").attr("state", 0);
            $(".multiple-select-table tr input[itemid=" + itemid + "]").parent().parent().find('.button[requireid=true]').addClass("disabled").attr("disabled", "disabled");
            for (var i = 0; i < selectedIdArray.length; i++) {
                if (selectedIdArray[i] == itemid) {
                    selectedIdArray.splice(i, 1);
                    break;
                }
            }
            selectedIds = "";
            for (var i = 0; i < selectedIdArray.length; i++) {
                selectedIds += selectedIdArray[i] + ",";
            }
            if (selectedIdArray.length > 0) {
                selectedId = selectedIdArray[0];
                $('#deleteSelected').removeAttr("disabled");
                $('.page-toolbars .button[requireid=true]').removeClass("disabled").removeAttr("disabled");
            } else {
                $('#deleteSelected').attr("disabled", "disabled");
                $('.page-toolbars .button[requireid=true]').addClass("disabled").attr("disabled", "disabled");
            }
            console.log("selectedId:" + selectedIds);
        }
    });
})

