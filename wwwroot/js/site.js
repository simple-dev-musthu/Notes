// Load incognito notes on page load
$(document).ready(function() {
    loadIncognitoNotes();
});

// Function to load incognito notes
function loadIncognitoNotes() {
    $.ajax({
        url: '/Notes/GetIncognitoNotes',
        type: 'GET',
        success: function(data) {
            updateIncognitoDropdown(data);
        },
        error: function() {
            console.log('Error loading incognito notes');
        }
    });
}

// Function to update the incognito dropdown
function updateIncognitoDropdown(notes) {
    var dropdown = $('#incognitoNotesList');
    var count = $('#incognitoCount');
    var noNotesMsg = $('#noIncognitoNotes');

    // Clear existing items (except header and divider)
    dropdown.find('.incognito-note-item').remove();

    if (notes && notes.length > 0) {
        count.text(notes.length);
        noNotesMsg.hide();

        notes.forEach(function(note) {
            var dateStr = new Date(note.updatedAt).toLocaleDateString('en-US', {
                month: 'short',
                day: 'numeric',
                year: 'numeric'
            });

            var item = `
                <li class="incognito-note-item">
                    <a class="dropdown-item" href="/Notes/VerifyPin?id=${note.id}&action=details">
                        <div class="d-flex justify-content-between align-items-start">
                            <div class="flex-grow-1">
                                <i class="fas fa-lock text-danger"></i> 
                                <strong>${escapeHtml(note.title)}</strong>
                            </div>
                        </div>
                        <small class="text-muted">
                            <i class="fas fa-clock"></i> ${dateStr}
                        </small>
                    </a>
                </li>
            `;
            dropdown.append(item);
        });
    } else {
        count.text('0');
        noNotesMsg.show();
    }
}

// Helper function to escape HTML
function escapeHtml(text) {
    var map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, function(m) { return map[m]; });
}

// Auto-dismiss alerts after 5 seconds
setTimeout(function() {
    $('.alert').fadeOut('slow');
}, 5000);
