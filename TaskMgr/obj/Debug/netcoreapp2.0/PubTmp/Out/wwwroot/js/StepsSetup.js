$(function () {
    $.ajaxSetup({ cache: false });
    var url = $("#pageUrl").val() + "/LoadData";

    $("#stepsTable").DataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ajax": {
            "url": url,
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs":
        [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { "data": "stepId", "name": "StepId", "autoWidth": true },
            { "data": "name", "name": "Name", "autoWidth": true },
            { "data": "class", "name": "Class", "autoWidth": true },
            { "data": "created", "name": "Created", "autoWidth": true },
            { "data": "modified", "name": "Modified", "autoWidth": true },
            {
                data: null, "orderable": false, render: function (data, type, row) {
                    return "<a href='#' class='btn-sm btn-success' onclick=AddEditData('" + row.stepId + "') >Edit</a>&nbsp&nbsp<a href='#' class='btn-sm btn-danger' onclick=DeleteData('" + row.stepId + "')>Delete</a> ";
                }
            }]
    });
});

function AddEditData(id) {

    var vm = {
        id: id
    };

    var url = $("#pageUrl").val() + "/AddEdit";

    $('#editPanel').load(url, vm, function (response, status, xhr) {
        if (status === "success") {
            $("form").each(function () { $.data($(this)[0], 'validator', false); });
            $.validator.unobtrusive.parse("form");

            $('#listPanel').hide();
            // call this when need to reload datatable
//            $('#stepsTable').DataTable().ajax.reload();
        }
    });
}

function DeleteData(id) {
    var vm = {
        id: id
    };

    var url = $("#pageUrl").val() + "/Delete";

    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: vm,
        dataType: "json",
        success: function (data) {
            $("#editPanel").html("");
            $('#listPanel').show();
            $('#stepsTable').DataTable().ajax.reload();
        }
    });
}

function Save(e) {

    var vm = {
        StepId: $('#StepId').val(),
        Name: $('#Name').val(),
        Class: $('#Class').val()
    }

    var url = $("#pageUrl").val() + "/Save";

    // somehow POST was not sending viewmodel to controller so used GET
    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: vm,
        dataType: "json",
        success: function (data) {
                $("#editPanel").html("");
                $('#listPanel').show();
                $('#stepsTable').DataTable().ajax.reload();
        }
    });

}

function Cancel(e) {

    $("#editPanel").html("");
    $('#listPanel').show();
}
