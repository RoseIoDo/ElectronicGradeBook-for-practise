$(document).ready(function () {
    loadSP();
    loadDropdownData(); // students, privileges

    $("#addSpBtn").click(function () {
        clearForm();
        $("#spModalLabel").text("Додати пільгу для студента");
        $("#spModal").modal("show");
    });

    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#spModalLabel").text("Редагувати пільгу");

        $("#spId").val($(this).data("id"));
        $("#studentSelect").val($(this).data("studentid"));
        $("#privilegeSelect").val($(this).data("privid"));
        $("#dateGranted").val($(this).data("granted"));
        $("#dateRevoked").val($(this).data("revoked") || "");

        $("#spModal").modal("show");
    });

    $("#saveSpBtn").click(function () {
        const idVal = parseInt($("#spId").val()) || 0;
        const stVal = parseInt($("#studentSelect").val()) || 0;
        const privVal = parseInt($("#privilegeSelect").val()) || 0;
        const grantedVal = $("#dateGranted").val() || null;
        const revokedVal = $("#dateRevoked").val() || null;

        const url = idVal ? "/StudentPrivileges/Edit" : "/StudentPrivileges/Add";
        const dataObj = {
            id: idVal,
            studentId: stVal,
            privilegeId: privVal,
            dateGranted: grantedVal,
            dateRevoked: revokedVal
        };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (resp) {
                if (resp.success) {
                    $("#spModal").modal("hide");
                    loadSP();
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка збереження привілею студента.");
            }
        });
    });

    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        if (confirm("Видалити?")) {
            $.ajax({
                url: `/StudentPrivileges/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        alert(resp.message);
                        loadSP();
                    } else {
                        alert(resp.message);
                    }
                },
                error: function () {
                    alert("Помилка видалення.");
                }
            });
        }
    });

    function loadSP() {
        $.ajax({
            url: "/StudentPrivileges/GetAll",
            type: "GET",
            success: function (list) {
                const $tbody = $("#spTable tbody");
                $tbody.empty();
                list.forEach(sp => {
                    const grantedStr = sp.dateGranted ? sp.dateGranted.split("T")[0] : "";
                    const revokedStr = sp.dateRevoked ? sp.dateRevoked.split("T")[0] : "";
                    $tbody.append(`
                        <tr>
                            <td>${sp.id}</td>
                            <td>${sp.studentName}</td>
                            <td>${sp.privilegeName}</td>
                            <td>${grantedStr}</td>
                            <td>${revokedStr}</td>
                            <td>
                                <button class="btn btn-warning btn-sm edit-btn"
                                    data-id="${sp.id}"
                                    data-studentid="${sp.studentId}"
                                    data-privid="${sp.privilegeId}"
                                    data-granted="${grantedStr}"
                                    data-revoked="${revokedStr || ""}">
                                    Редагувати
                                </button>
                                <button class="btn btn-danger btn-sm delete-btn"
                                    data-id="${sp.id}">
                                    Видалити
                                </button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function () {
                alert("Помилка завантаження студентських привілеїв.");
            }
        });
    }

    function loadDropdownData() {
        // /Students/GetAll -> fill #studentSelect
        // /Privileges/GetAll -> fill #privilegeSelect
    }

    function clearForm() {
        $("#spId").val("");
        $("#studentSelect").val("");
        $("#privilegeSelect").val("");
        $("#dateGranted").val("");
        $("#dateRevoked").val("");
    }
});
