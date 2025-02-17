$(document).ready(function () {
    loadPrivileges();

    $("#addPrivilegeBtn").click(function () {
        clearForm();
        $("#privilegeModalLabel").text("Додати привілей");
        $("#privilegeModal").modal("show");
    });

    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#privilegeModalLabel").text("Редагувати привілей");

        $("#privilegeId").val($(this).data("id"));
        $("#pName").val($(this).data("name"));
        $("#pDesc").val($(this).data("desc"));

        $("#privilegeModal").modal("show");
    });

    $("#savePrivilegeBtn").click(function () {
        const idVal = parseInt($("#privilegeId").val()) || 0;
        const nameVal = $("#pName").val().trim();
        const descVal = $("#pDesc").val().trim();

        if (!nameVal) {
            alert("Назва не може бути пустою.");
            return;
        }

        const url = idVal ? "/Privileges/Edit" : "/Privileges/Add";
        const dataObj = {
            id: idVal,
            name: nameVal,
            description: descVal
        };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (resp) {
                if (resp.success) {
                    $("#privilegeModal").modal("hide");
                    loadPrivileges();
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка збереження привілею.");
            }
        });
    });

    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        if (confirm("Видалити привілей?")) {
            $.ajax({
                url: `/Privileges/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        alert(resp.message);
                        loadPrivileges();
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

    function loadPrivileges() {
        $.ajax({
            url: "/Privileges/GetAll",
            type: "GET",
            success: function (list) {
                const $tbody = $("#privilegesTable tbody");
                $tbody.empty();
                list.forEach(p => {
                    $tbody.append(`
                        <tr>
                            <td>${p.id}</td>
                            <td>${p.name}</td>
                            <td>${p.description || ""}</td>
                            <td>
                                <button class="btn btn-warning btn-sm edit-btn"
                                    data-id="${p.id}"
                                    data-name="${p.name}"
                                    data-desc="${p.description || ""}">
                                    Редагувати
                                </button>
                                <button class="btn btn-danger btn-sm delete-btn"
                                    data-id="${p.id}">
                                    Видалити
                                </button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function () {
                alert("Помилка завантаження привілеїв.");
            }
        });
    }

    function clearForm() {
        $("#privilegeId").val("");
        $("#pName").val("");
        $("#pDesc").val("");
    }
});
