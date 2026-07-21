/*
 * -------------------------------------------------------
 * HTML-ELEMENTE
 * -------------------------------------------------------
 */

/* Allgemeines Board */

const messageElement =
    document.querySelector("#message");

const reloadButton =
    document.querySelector("#reloadButton");


/* Ticket erstellen */

const createTicketButton =
    document.querySelector("#createTicketButton");

const createTicketDialog =
    document.querySelector("#createTicketDialog");

const closeTicketDialogButton =
    document.querySelector("#closeTicketDialogButton");

const cancelCreateTicketButton =
    document.querySelector("#cancelCreateTicketButton");

const createTicketForm =
    document.querySelector("#createTicketForm");

const prioritySelect =
    document.querySelector("#ticketPriority");

const createTicketError =
    document.querySelector("#createTicketError");

const submitTicketButton =
    document.querySelector("#submitTicketButton");

const ticketTitleInput =
    document.querySelector("#ticketTitle");

const ticketCreatedByInput =
    document.querySelector("#ticketCreatedBy");


/* Ticketdetails */

const ticketDetailsDialog =
    document.querySelector("#ticketDetailsDialog");

const ticketDetailsHeading =
    document.querySelector("#ticketDetailsHeading");

const closeTicketDetailsButton =
    document.querySelector("#closeTicketDetailsButton");

const cancelTicketDetailsButton =
    document.querySelector("#cancelTicketDetailsButton");

const ticketDetailsForm =
    document.querySelector("#ticketDetailsForm");

const detailsTicketId =
    document.querySelector("#detailsTicketId");

const detailsTitle =
    document.querySelector("#detailsTitle");

const detailsDescription =
    document.querySelector("#detailsDescription");

const detailsStatus =
    document.querySelector("#detailsStatus");

const detailsPriority =
    document.querySelector("#detailsPriority");

const detailsCreatedBy =
    document.querySelector("#detailsCreatedBy");

const detailsCreatedAt =
    document.querySelector("#detailsCreatedAt");

const detailsUpdatedAt =
    document.querySelector("#detailsUpdatedAt");

const ticketDetailsError =
    document.querySelector("#ticketDetailsError");

const deleteTicketButton =
    document.querySelector("#deleteTicketButton");

const saveTicketButton =
    document.querySelector("#saveTicketButton");


/*
 * Aktuell geladene Tickets.
 */
let currentTickets = [];


/*
 * -------------------------------------------------------
 * EVENT-LISTENER
 * -------------------------------------------------------
 */

reloadButton.addEventListener(
    "click",
    loadTickets
);


/* Ticket erstellen */

createTicketButton.addEventListener(
    "click",
    openCreateTicketDialog
);

closeTicketDialogButton.addEventListener(
    "click",
    closeCreateTicketDialog
);

cancelCreateTicketButton.addEventListener(
    "click",
    closeCreateTicketDialog
);

createTicketForm.addEventListener(
    "submit",
    createTicket
);

createTicketDialog.addEventListener(
    "click",
    event => {
        if (event.target === createTicketDialog) {
            closeCreateTicketDialog();
        }
    }
);


/* Ticketdetails */

closeTicketDetailsButton.addEventListener(
    "click",
    closeTicketDetails
);

cancelTicketDetailsButton.addEventListener(
    "click",
    closeTicketDetails
);

ticketDetailsForm.addEventListener(
    "submit",
    saveTicketChanges
);

deleteTicketButton.addEventListener(
    "click",
    deleteSelectedTicket
);

ticketDetailsDialog.addEventListener(
    "click",
    event => {
        if (event.target === ticketDetailsDialog) {
            closeTicketDetails();
        }
    }
);


/*
 * Anwendung starten.
 */
setupDropZones();
loadTickets();


/*
 * -------------------------------------------------------
 * TICKET ERSTELLEN
 * -------------------------------------------------------
 */

async function openCreateTicketDialog() {
    createTicketError.textContent = "";

    await loadPriorities();

    createTicketDialog.showModal();
    ticketTitleInput.focus();
}


function closeCreateTicketDialog() {
    createTicketDialog.close();
}


async function loadPriorities() {
    /*
     * Wenn bereits Prioritäten im Dropdown stehen,
     * nicht noch einmal laden.
     */
    if (prioritySelect.options.length > 1) {
        return;
    }

    try {
        const response =
            await fetch("/tickets/priorities");

        if (!response.ok) {
            throw new Error(
                `HTTP-Status ${response.status}`
            );
        }

        const priorities =
            await response.json();

        for (const priority of priorities) {
            const option =
                document.createElement("option");

            option.value =
                priority.id;

            option.textContent =
                priority.name;

            prioritySelect.append(option);
        }
    } catch (error) {
        console.error(error);

        createTicketError.textContent =
            "Prioritäten konnten nicht geladen werden.";
    }
}


async function createTicket(event) {
    event.preventDefault();

    createTicketError.textContent = "";

    submitTicketButton.disabled = true;
    submitTicketButton.textContent =
        "Wird erstellt …";

    const formData =
        new FormData(createTicketForm);

    const requestBody = {
        title:
            formData.get("title").trim(),

        description:
            formData.get("description").trim(),

        priorityId:
            Number(formData.get("priorityId")),

        createdBy:
            formData.get("createdBy").trim()
    };

    try {
        const response = await fetch(
            "/tickets",
            {
                method: "POST",

                headers: {
                    "Content-Type":
                        "application/json"
                },

                body: JSON.stringify(requestBody)
            }
        );

        if (!response.ok) {
            throw new Error(
                await readErrorMessage(response)
            );
        }

        const createdTicket =
            await response.json();

        createTicketDialog.close();
        createTicketForm.reset();

        /*
         * Nach reset() wieder Testbenutzer setzen.
         */
        ticketCreatedByInput.value = "Semih";

        await loadTickets();

        messageElement.textContent =
            `Ticket #${createdTicket.id} wurde erstellt.`;
    } catch (error) {
        console.error(error);

        createTicketError.textContent =
            error.message;
    } finally {
        submitTicketButton.disabled = false;

        submitTicketButton.textContent =
            "Ticket erstellen";
    }
}


/*
 * -------------------------------------------------------
 * TICKETS LADEN
 * -------------------------------------------------------
 */

async function loadTickets() {
    messageElement.textContent =
        "Tickets werden geladen …";

    clearBoard();

    try {
        const response =
            await fetch("/tickets");

        if (!response.ok) {
            throw new Error(
                `API antwortet mit Status ${response.status}`
            );
        }

        currentTickets =
            await response.json();

        updateCounters(currentTickets);
        renderTickets(currentTickets);

        messageElement.textContent =
            `${currentTickets.length} Tickets geladen.`;
    } catch (error) {
        console.error(error);

        messageElement.textContent =
            "Tickets konnten nicht geladen werden.";
    }
}


function clearBoard() {
    const ticketLists =
        document.querySelectorAll(".ticket-list");

    for (const ticketList of ticketLists) {
        ticketList.replaceChildren();
    }
}


/*
 * -------------------------------------------------------
 * ZÄHLER
 * -------------------------------------------------------
 */

function updateCounters(tickets) {
    const totalCount =
        tickets.length;

    const openCount =
        tickets.filter(
            ticket => ticket.statusId === 1
        ).length;

    const progressCount =
        tickets.filter(
            ticket => ticket.statusId === 2
        ).length;

    /*
     * Resolved und Closed zählen als erledigt.
     */
    const resolvedCount =
        tickets.filter(
            ticket =>
                ticket.statusId === 3 ||
                ticket.statusId === 4
        ).length;

    document.querySelector(
        "#totalCount"
    ).textContent = totalCount;

    document.querySelector(
        "#openCount"
    ).textContent = openCount;

    document.querySelector(
        "#progressCount"
    ).textContent = progressCount;

    document.querySelector(
        "#resolvedCount"
    ).textContent = resolvedCount;
}


/*
 * -------------------------------------------------------
 * TICKETKARTEN DARSTELLEN
 * -------------------------------------------------------
 */

function renderTickets(tickets) {
    for (const ticket of tickets) {
        const ticketList =
            document.querySelector(
                `#status-${ticket.statusId}`
            );

        if (!ticketList) {
            continue;
        }

        const card =
            createTicketCard(ticket);

        ticketList.append(card);
    }
}


function createTicketCard(ticket) {
    const card =
        document.createElement("article");

    card.className = "ticket-card";
    card.draggable = true;
    card.dataset.ticketId = ticket.id;

    let cardWasDragged = false;


    const title =
        document.createElement("h3");

    title.textContent =
        `#${ticket.id} ${ticket.title}`;


    const description =
        document.createElement("p");

    description.textContent =
        ticket.description;


    const metadata =
        document.createElement("small");

    const priorityName =
        ticket.priority?.name ??
        `Priorität ${ticket.priorityId}`;

    metadata.textContent =
        `${priorityName} · Erstellt von ${ticket.createdBy}`;


    card.append(
        title,
        description,
        metadata
    );


    /*
     * Detailfenster öffnen.
     */
    card.addEventListener(
        "click",
        () => {
            if (cardWasDragged) {
                return;
            }

            openTicketDetails(ticket.id);
        }
    );


    /*
     * Drag starten.
     */
    card.addEventListener(
        "dragstart",
        event => {
            cardWasDragged = true;

            card.classList.add("dragging");

            event.dataTransfer.setData(
                "text/plain",
                ticket.id.toString()
            );

            event.dataTransfer.effectAllowed =
                "move";
        }
    );


    /*
     * Drag beenden.
     */
    card.addEventListener(
        "dragend",
        () => {
            card.classList.remove("dragging");

            const columns =
                document.querySelectorAll(".column");

            for (const column of columns) {
                column.classList.remove(
                    "drag-over"
                );
            }

            window.setTimeout(
                () => {
                    cardWasDragged = false;
                },
                150
            );
        }
    );


    return card;
}


/*
 * -------------------------------------------------------
 * DRAG AND DROP
 * -------------------------------------------------------
 */

function setupDropZones() {
    const columns =
        document.querySelectorAll(".column");

    for (const column of columns) {
        column.addEventListener(
            "dragover",
            event => {
                event.preventDefault();

                event.dataTransfer.dropEffect =
                    "move";

                column.classList.add(
                    "drag-over"
                );
            }
        );


        column.addEventListener(
            "dragleave",
            event => {
                if (
                    !column.contains(
                        event.relatedTarget
                    )
                ) {
                    column.classList.remove(
                        "drag-over"
                    );
                }
            }
        );


        column.addEventListener(
            "drop",
            async event => {
                event.preventDefault();

                column.classList.remove(
                    "drag-over"
                );

                const ticketId =
                    Number(
                        event.dataTransfer.getData(
                            "text/plain"
                        )
                    );

                const newStatusId =
                    Number(
                        column.dataset.statusId
                    );

                await moveTicket(
                    ticketId,
                    newStatusId,
                    column
                );
            }
        );
    }
}


async function moveTicket(
    ticketId,
    newStatusId,
    targetColumn
) {
    const ticket =
        currentTickets.find(
            currentTicket =>
                currentTicket.id === ticketId
        );

    if (!ticket) {
        messageElement.textContent =
            `Ticket #${ticketId} wurde nicht gefunden.`;

        return;
    }

    if (ticket.statusId === newStatusId) {
        messageElement.textContent =
            `Ticket #${ticketId} befindet sich bereits in dieser Spalte.`;

        return;
    }

    const oldStatusId =
        ticket.statusId;

    const card =
        document.querySelector(
            `[data-ticket-id="${ticketId}"]`
        );

    const targetList =
        targetColumn.querySelector(
            ".ticket-list"
        );

    /*
     * Karte sofort sichtbar verschieben.
     */
    ticket.statusId = newStatusId;

    if (card && targetList) {
        targetList.append(card);
    }

    updateCounters(currentTickets);

    messageElement.textContent =
        `Ticket #${ticketId} wird gespeichert …`;

    try {
        const response = await fetch(
            `/tickets/${ticketId}`,
            {
                method: "PUT",

                headers: {
                    "Content-Type":
                        "application/json"
                },

                body: JSON.stringify({
                    statusId: newStatusId
                })
            }
        );

        if (!response.ok) {
            throw new Error(
                await readErrorMessage(response)
            );
        }

        const updatedTicket =
            await response.json();

        Object.assign(
            ticket,
            updatedTicket
        );

        messageElement.textContent =
            `Ticket #${ticketId} wurde gespeichert.`;
    } catch (error) {
        console.error(error);

        ticket.statusId =
            oldStatusId;

        messageElement.textContent =
            `Ticket #${ticketId} konnte nicht gespeichert werden.`;

        await loadTickets();
    }
}


/*
 * -------------------------------------------------------
 * TICKETDETAILS LADEN
 * -------------------------------------------------------
 */

async function openTicketDetails(ticketId) {
    ticketDetailsError.textContent = "";

    try {
        const [
            ticketResponse,
            statusesResponse,
            prioritiesResponse
        ] = await Promise.all([
            fetch(`/tickets/${ticketId}`),
            fetch("/tickets/statuses"),
            fetch("/tickets/priorities")
        ]);

        if (!ticketResponse.ok) {
            throw new Error(
                "Ticket konnte nicht geladen werden."
            );
        }

        if (!statusesResponse.ok) {
            throw new Error(
                "Statuswerte konnten nicht geladen werden."
            );
        }

        if (!prioritiesResponse.ok) {
            throw new Error(
                "Prioritäten konnten nicht geladen werden."
            );
        }

        const ticket =
            await ticketResponse.json();

        const statuses =
            await statusesResponse.json();

        const priorities =
            await prioritiesResponse.json();


        detailsTicketId.value =
            ticket.id;

        ticketDetailsHeading.textContent =
            `Ticket #${ticket.id}`;

        detailsTitle.value =
            ticket.title;

        detailsDescription.value =
            ticket.description;

        fillSelect(
            detailsStatus,
            statuses,
            ticket.statusId
        );

        fillSelect(
            detailsPriority,
            priorities,
            ticket.priorityId
        );

        detailsCreatedBy.textContent =
            ticket.createdBy;

        detailsCreatedAt.textContent =
            formatDate(ticket.createdAt);

        detailsUpdatedAt.textContent =
            ticket.updatedAt
                ? formatDate(ticket.updatedAt)
                : "Noch nicht geändert";

        ticketDetailsDialog.showModal();
    } catch (error) {
        console.error(error);

        messageElement.textContent =
            error.message;
    }
}


function fillSelect(
    selectElement,
    values,
    selectedId
) {
    selectElement.replaceChildren();

    for (const value of values) {
        const option =
            document.createElement("option");

        option.value =
            value.id;

        option.textContent =
            value.name;

        option.selected =
            value.id === selectedId;

        selectElement.append(option);
    }
}


function closeTicketDetails() {
    ticketDetailsDialog.close();
    ticketDetailsError.textContent = "";
}


/*
 * -------------------------------------------------------
 * TICKET BEARBEITEN
 * -------------------------------------------------------
 */

async function saveTicketChanges(event) {
    event.preventDefault();

    ticketDetailsError.textContent = "";

    saveTicketButton.disabled = true;

    saveTicketButton.textContent =
        "Wird gespeichert …";

    const ticketId =
        Number(detailsTicketId.value);

    const requestBody = {
        title:
            detailsTitle.value.trim(),

        description:
            detailsDescription.value.trim(),

        statusId:
            Number(detailsStatus.value),

        priorityId:
            Number(detailsPriority.value)
    };

    try {
        const response = await fetch(
            `/tickets/${ticketId}`,
            {
                method: "PUT",

                headers: {
                    "Content-Type":
                        "application/json"
                },

                body: JSON.stringify(requestBody)
            }
        );

        if (!response.ok) {
            throw new Error(
                await readErrorMessage(response)
            );
        }

        ticketDetailsDialog.close();

        await loadTickets();

        messageElement.textContent =
            `Ticket #${ticketId} wurde aktualisiert.`;
    } catch (error) {
        console.error(error);

        ticketDetailsError.textContent =
            error.message;
    } finally {
        saveTicketButton.disabled = false;

        saveTicketButton.textContent =
            "Änderungen speichern";
    }
}


/*
 * -------------------------------------------------------
 * TICKET LÖSCHEN
 * -------------------------------------------------------
 */

async function deleteSelectedTicket() {
    const ticketId =
        Number(detailsTicketId.value);

    const confirmed =
        window.confirm(
            `Soll Ticket #${ticketId} wirklich gelöscht werden?`
        );

    if (!confirmed) {
        return;
    }

    ticketDetailsError.textContent = "";

    deleteTicketButton.disabled = true;

    deleteTicketButton.textContent =
        "Wird gelöscht …";

    try {
        const response = await fetch(
            `/tickets/${ticketId}`,
            {
                method: "DELETE"
            }
        );

        if (!response.ok) {
            throw new Error(
                await readErrorMessage(response)
            );
        }

        ticketDetailsDialog.close();

        await loadTickets();

        messageElement.textContent =
            `Ticket #${ticketId} wurde gelöscht.`;
    } catch (error) {
        console.error(error);

        ticketDetailsError.textContent =
            error.message;
    } finally {
        deleteTicketButton.disabled = false;

        deleteTicketButton.textContent =
            "Ticket löschen";
    }
}


/*
 * -------------------------------------------------------
 * HILFSFUNKTIONEN
 * -------------------------------------------------------
 */

async function readErrorMessage(response) {
    try {
        const errorBody =
            await response.json();

        return (
            errorBody.message ??
            `HTTP-Status ${response.status}`
        );
    } catch {
        return `HTTP-Status ${response.status}`;
    }
}


function formatDate(dateValue) {
    const date =
        new Date(dateValue);

    return new Intl.DateTimeFormat(
        "de-DE",
        {
            dateStyle: "medium",
            timeStyle: "short"
        }
    ).format(date);
}