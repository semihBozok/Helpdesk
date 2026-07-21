const messageElement = document.querySelector("#message");
const reloadButton = document.querySelector("#reloadButton");

let currentTickets = [];

reloadButton.addEventListener("click", loadTickets);

setupDropZones();
loadTickets();


async function loadTickets() {
    messageElement.textContent = "Tickets werden geladen …";

    clearBoard();

    try {
        const response = await fetch("/tickets");

        if (!response.ok) {
            throw new Error(
                `API antwortet mit Status ${response.status}`
            );
        }

        currentTickets = await response.json();

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


function updateCounters(tickets) {
    document.querySelector("#totalCount").textContent =
        tickets.length;

    document.querySelector("#openCount").textContent =
        tickets.filter(
            ticket => ticket.statusId === 1
        ).length;

    document.querySelector("#progressCount").textContent =
        tickets.filter(
            ticket => ticket.statusId === 2
        ).length;

    document.querySelector("#resolvedCount").textContent =
        tickets.filter(
            ticket => ticket.statusId === 3
        ).length;
}


function renderTickets(tickets) {
    for (const ticket of tickets) {
        const column = document.querySelector(
            `#status-${ticket.statusId}`
        );

        if (!column) {
            continue;
        }

        const card = createTicketCard(ticket);

        column.append(card);
    }
}


function createTicketCard(ticket) {
    const card = document.createElement("article");

    card.className = "ticket-card";

    // Macht die Karte mit der Maus verschiebbar.
    card.draggable = true;

    // Speichert die Ticket-ID direkt am HTML-Element.
    card.dataset.ticketId = ticket.id;


    const title = document.createElement("h3");

    title.textContent =
        `#${ticket.id} ${ticket.title}`;


    const description = document.createElement("p");

    description.textContent = ticket.description;


    const metadata = document.createElement("small");

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


    card.addEventListener("dragstart", event => {
        card.classList.add("dragging");

        event.dataTransfer.setData(
            "text/plain",
            ticket.id.toString()
        );

        event.dataTransfer.effectAllowed = "move";
    });


    card.addEventListener("dragend", () => {
        card.classList.remove("dragging");
    });


    return card;
}


function setupDropZones() {
    const dropZones =
        document.querySelectorAll(".ticket-list");

    for (const dropZone of dropZones) {
        dropZone.addEventListener("dragover", event => {
            // Ohne preventDefault wäre Ablegen nicht erlaubt.
            event.preventDefault();

            event.dataTransfer.dropEffect = "move";

            dropZone.classList.add("drag-over");
        });


        dropZone.addEventListener("dragleave", event => {
            /*
             * dragleave kann auch ausgelöst werden,
             * wenn man nur über ein Kind-Element fährt.
             */
            if (!dropZone.contains(event.relatedTarget)) {
                dropZone.classList.remove("drag-over");
            }
        });


        dropZone.addEventListener("drop", async event => {
            event.preventDefault();

            dropZone.classList.remove("drag-over");

            const ticketId = Number(
                event.dataTransfer.getData("text/plain")
            );

            const newStatusId = Number(
                dropZone.dataset.statusId
            );

            await updateTicketStatus(
                ticketId,
                newStatusId
            );
        });
    }
}


async function updateTicketStatus(
    ticketId,
    newStatusId
) {
    const ticket = currentTickets.find(
        currentTicket =>
            currentTicket.id === ticketId
    );

    if (!ticket) {
        messageElement.textContent =
            "Das verschobene Ticket wurde nicht gefunden.";

        return;
    }

    if (ticket.statusId === newStatusId) {
        messageElement.textContent =
            `Ticket #${ticketId} befindet sich bereits in dieser Spalte.`;

        return;
    }

    messageElement.textContent =
        `Ticket #${ticketId} wird aktualisiert …`;

    try {
        const response = await fetch(
            `/tickets/${ticketId}`,
            {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    statusId: newStatusId
                })
            }
        );

        if (!response.ok) {
            const errorBody = await response.text();

            throw new Error(
                `Status ${response.status}: ${errorBody}`
            );
        }

        const updatedTicket = await response.json();

        messageElement.textContent =
            `Ticket #${updatedTicket.id} wurde verschoben.`;

        /*
         * Wir laden erneut aus PostgreSQL.
         * Dadurch zeigt das Board garantiert den
         * tatsächlich gespeicherten Zustand.
         */
        await loadTickets();
    } catch (error) {
        console.error(error);

        messageElement.textContent =
            `Ticket #${ticketId} konnte nicht verschoben werden.`;

        // Ursprünglichen DB-Zustand erneut anzeigen.
        await loadTickets();
    }
}