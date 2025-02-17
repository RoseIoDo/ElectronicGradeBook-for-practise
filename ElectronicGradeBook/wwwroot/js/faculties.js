$(document).ready(function () {

    loadFaculties();

    $("#addFacultyBtn").click(function () {
        clearForm();
        $("#facultyModalLabel").text("Додати факультет");
        $("#facultyModal").modal("show");
    });

    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#facultyModalLabel").text("Редагувати факультет");

        $("#facultyId").val($(this).data("id"));
        $("#facultyName").val($(this).data("name"));

        $("#facultyModal").modal("show");
    });

    $("#saveFacultyBtn").click(function () {
        const idVal = parseInt($("#facultyId").val()) || 0;
        const nameVal = $("#facultyName").val().trim();

        if (!nameVal) {
            alert("Назва факультету не може бути порожньою.");
            return;
        }

        const url = idVal ? "/Faculties/Edit" : "/Faculties/Add";
        const dataObj = { id: idVal, name: nameVal };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (resp) {
                if (resp.success) {
                    $("#facultyModal").modal("hide");
                    loadFaculties();
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка на сервері (Add/Edit).");
            }
        });
    });

    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        if (confirm("Видалити факультет?")) {
            $.ajax({
                url: `/Faculties/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        alert(resp.message);
                        loadFaculties();
                    } else {
                        alert(resp.message);
                    }
                },
                error: function () {
                    alert("Помилка видалення факультету.");
                }
            });
        }
    });

    function loadFaculties() {
        $.ajax({
            url: "/Faculties/GetAll",
            type: "GET",
            success: function (list) {
                const $tbody = $("#facultiesTable tbody");
                $tbody.empty();
                list.forEach(f => {
                    $tbody.append(`
                        <tr>
                            <td>${f.id}</td>
                            <td>${f.name}</td>
                            <td>
                                <button class="btn btn-warning btn-sm edit-btn"
                                        data-id="${f.id}"
                                        data-name="${f.name}">
                                    Редагувати
                                </button>
                                <button class="btn btn-danger btn-sm delete-btn"
                                        data-id="${f.id}">
                                    Видалити
                                </button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function () {
                alert("Помилка завантаження факультетів.");
            }
        });
    }

    function clearForm() {
        $("#facultyId").val("");
        $("#facultyName").val("");
    }
});
