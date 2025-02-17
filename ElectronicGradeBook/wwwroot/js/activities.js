$(document).ready(function () {
    loadActivities();

    $("#addActivityBtn").click(function () {
        clearForm();
        $("#activityModalLabel").text("Додати активність");
        $("#activityModal").modal("show");
    });

    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#activityModalLabel").text("Редагувати активність");

        $("#activityId").val($(this).data("id"));
        $("#activityName").val($(this).data("name"));
        $("#points").val($(this).data("points"));
        $("#type").val($(this).data("type"));
        $("#description").val($(this).data("desc"));

        $("#activityModal").modal("show");
    });

    $("#saveActivityBtn").click(function () {
        const idVal = parseInt($("#activityId").val()) || 0;
        const nameVal = $("#activityName").val().trim();
        const pointsVal = parseInt($("#points").val()) || 0;
        const typeVal = $("#type").val().trim();
        const descVal = $("#description").val().trim();

        if (!nameVal) {
            alert("Назва активності не може бути пустою.");
            return;
        }

        const url = idVal ? "/Activities/Edit" : "/Activities/Add";
        const dataObj = {
            id: idVal,
            name: nameVal,
            points: pointsVal,
            type: typeVal,
            description: descVal
        };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (resp) {
                if (resp.success) {
                    $("#activityModal").modal("hide");
                    loadActivities();
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка збереження активності.");
            }
        });
    });

    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        if (confirm("Видалити активність?")) {
            $.ajax({
                url: `/Activities/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        alert(resp.message);
                        loadActivities();
                    } else {
                        alert(resp.message);
                    }
                },
                error: function () {
                    alert("Помилка при видаленні.");
                }
            });
        }
    });

    function loadActivities() {
        $.ajax({
            url: "/Activities/GetAll",
            type: "GET",
            success: function (list) {
                const $tbody = $("#activitiesTable tbody");
                $tbody.empty();
                list.forEach(a => {
                    $tbody.append(`
                        <tr>
                            <td>${a.id}</td>
                            <td>${a.name}</td>
                            <td>${a.points}</td>
                            <td>${a.type || ""}</td>
                            <td>${a.description || ""}</td>
                            <td>
                                <button class="btn btn-warning btn-sm edit-btn"
                                    data-id="${a.id}"
                                    data-name="${a.name}"
                                    data-points="${a.points}"
                                    data-type="${a.type}"
                                    data-desc="${a.description || ""}">
                                    Редагувати
                                </button>
                                <button class="btn btn-danger btn-sm delete-btn"
                                    data-id="${a.id}">
                                    Видалити
                                </button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function () {
                alert("Помилка завантаження активностей.");
            }
        });
    }

    function clearForm() {
        $("#activityId").val("");
        $("#activityName").val("");
        $("#points").val("");
        $("#type").val("");
        $("#description").val("");
    }
});
