$(document).ready(function () {
    console.log("teachers.js loaded");

    loadTeachers();

    $("#addTeacherBtn").click(function () {
        clearForm();
        $("#teacherModalLabel").text("Додати викладача");
        $("#teacherModal").modal("show");
    });

    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#teacherModalLabel").text("Редагувати викладача");

        $("#teacherId").val($(this).data("id"));
        $("#fullName").val($(this).data("name"));
        $("#position").val($(this).data("position"));
        $("#userId").val($(this).data("userid")); // опціонально

        $("#teacherModal").modal("show");
    });

    $("#saveTeacherBtn").click(function () {
        const idVal = parseInt($("#teacherId").val()) || 0;
        const nameVal = $("#fullName").val().trim();
        const posVal = $("#position").val().trim();
        const userVal = parseInt($("#userId").val()) || null; // якщо потрібно

        if (!nameVal) {
            alert("ПІБ викладача не може бути пустим.");
            return;
        }

        const url = idVal ? "/Teachers/Edit" : "/Teachers/Add";
        const dataObj = {
            id: idVal,
            fullName: nameVal,
            position: posVal,
            userId: userVal
        };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dataObj),
            success: function (resp) {
                if (resp.success) {
                    $("#teacherModal").modal("hide");
                    loadTeachers();
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка збереження викладача.");
            }
        });
    });

    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");
        if (confirm("Видалити викладача?")) {
            $.ajax({
                url: `/Teachers/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        alert(resp.message);
                        loadTeachers();
                    } else {
                        alert(resp.message);
                    }
                },
                error: function () {
                    alert("Помилка видалення викладача.");
                }
            });
        }
    });

    function loadTeachers() {
        $.ajax({
            url: "/Teachers/GetAll",
            type: "GET",
            success: function (list) {
                const $tbody = $("#teachersTable tbody");
                $tbody.empty();
                list.forEach(t => {
                    // Формуємо рядок із 4 колонками: ID, ПІБ, Посада, Дії
                    $tbody.append(`
                        <tr>
                            <td>${t.id}</td>
                            <td>${t.fullName}</td>
                            <td>${t.position}</td>
                            <td>
                                <button class="btn btn-warning btn-sm edit-btn"
                                    data-id="${t.id}"
                                    data-name="${t.fullName}"
                                    data-position="${t.position}"
                                    data-userid="${t.userId || ""}">
                                    Редагувати
                                </button>
                                <button class="btn btn-danger btn-sm delete-btn" data-id="${t.id}">
                                    Видалити
                                </button>
                            </td>
                        </tr>
                    `);
                });
            },
            error: function () {
                alert("Помилка завантаження викладачів.");
            }
        });
    }

    function clearForm() {
        $("#teacherId").val("");
        $("#fullName").val("");
        $("#position").val("");
        $("#userId").val("");
    }
});
