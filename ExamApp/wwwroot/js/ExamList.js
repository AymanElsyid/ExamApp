function Delete(Id) {


    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3f51b5',
        cancelButtonColor: '#ff4081',
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancel',
        customClass: {
            confirmButton: "btn btn-primary",
            cancelButton: "btn btn-danger"
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // Send AJAX request when confirmed
            $.ajax({
                url: '/Admin/Exam/Delete', // Update with your API/controller action
                type: 'POST', // Change to GET if needed
                data: { id: Id }, // Pass necessary data
                success: function (response) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'Your action was completed successfully.',
                        icon: 'success'
                    });
                    location.reload();
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        title: 'Error!',
                        text: 'Something went wrong. Please try again.',
                        icon: 'error'
                    });
                    console.error("AJAX Error:", error);
                }
            });
        } else {
            console.log("Action Cancelled");
        }
    });
};
function Active(Id) {


    Swal.fire({
        title: 'Activate Exam?',
        text: "Are you sure you want to activate this exam?",
        icon: 'info', // Changed from 'warning' to 'info' to indicate activation
        showCancelButton: true,
        confirmButtonColor: '#28a745', // Green color for activation
        cancelButtonColor: '#ff4081',
        confirmButtonText: 'Activate',
        cancelButtonText: 'Cancel',
        customClass: {
            confirmButton: "btn btn-success", // Green Bootstrap button
            cancelButton: "btn btn-danger"
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // Send AJAX request when confirmed
            $.ajax({
                url: '/Admin/Exam/Active', // Update with actual endpoint
                type: 'POST',
                data: { id: Id }, // Pass necessary data
                success: function (response) {
                    Swal.fire({
                        title: 'Activated!',
                        text: 'The exam has been successfully activated.',
                        icon: 'success'
                    }).then(() => {
                        // Redirect after confirmation
                        location.reload();
                    });
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        title: 'Error!',
                        text: 'Something went wrong. Please try again.',
                        icon: 'error'
                    });
                    console.error("AJAX Error:", error);
                }
            });
        } else {
            console.log("Exam activation cancelled.");
        }
    });
}