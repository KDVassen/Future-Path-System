# Future Path — screen standards (spec Section 2.2)

Applies to every screen, using standard .NET Framework Windows Forms controls with no custom
painting libraries. The spec wording wins unless `decisions.md` records a change (D6–D13). The hex
values are the approved "Academic Navy Rail" palette the spec's colour names refer to.

Pixel sizes below are at 100% Windows scaling (96 DPI). `StaffConsoleForm.Designer.cs` stores
them at 96 DPI; `MainForm.Designer.cs` was last saved by Visual Studio at 125% (120 DPI), so its
numbers are 1.25 × these. Both look the same at run time because the app is DPI-aware (D13).

## Window layout (2.2.1, decisions D6–D10)

- `FormBorderStyle = FixedSingle`, `MaximizeBox = false`, `StartPosition = CenterScreen`,
  `AutoScaleMode = Dpi`.
- **Main windows** (`MainForm`, `StaffConsoleForm`): client **974 × 761** (about 990 × 800 with the
  frame). **Dialogs** (UI 2.1a, 3.1a, 8.1a, confirmations, staff login): **520 × 300**, with a navy
  header band.
- **Zones in a main window, top to bottom:**
  1. **Caption bar:** "Future Path — <tab name>".
  2. **Menu strip** (`mnuMain`, Segoe UI 10): learner **Home | Exit**; staff **Sign out | Exit**.
  3. **Tab control** (`tcMain` / `tcConsole`, Segoe UI 10, `Padding (6, 5)`) inside `pnlBody`
     (background #F4F6F9, padding 16/8/16/7). A tab that groups several screens holds its own
     tab control (`tcResults`, `tcSessions`, `tcReference`).
  4. **Status bar** (`pnlStatus`, deep navy, 26 high): the message on the left, the learner or
     signed-in user on the right.
- **Inside every tab, top to bottom:**
  1. **Notice label** (`lbl<Tab>Notice`, Dock Top, 48 high, Segoe UI 10): learner tabs that need
     guidance only. Leave a 10 px gap below it.
  2. **Content panel** (`pnl<Tab>Content`, Dock Fill): white cards on the page background.
  3. **Button row** (`flp<Tab>Actions`, Dock Bottom, 64 high, `FlowDirection = RightToLeft`).
- **Conditional zones** (notice text, validation summaries, tiles) are **hidden, never shown empty**.
- **Cards:** white `Panel`, `BorderStyle.FixedSingle`, 12 px apart. Amber heading bar
  (`pnl<Card>HeadingBar`) 4 × 20 at (16, 12); heading label at (26, 8). Fields start 16–20 px in
  from the card edge; keep every field in one card on the same left edge. Count captions
  ("6 of 7 captured") sit top-right of the heading row.

## Navigation (2.2.2, decisions D7, D10)

- The **tab headers** are the main way to move between areas (D7 replaces the rail).
- **Learner steps run in order:** `tcMain_Selecting` cancels a step tab not yet reached, with an
  amber status message. **Home** (tab, menu, Cancel, or Back on step 1) asks before leaving the
  session.
- **Staff areas** can be opened in any order. The Report viewer tab opens only after a report is
  previewed. Tabs a role may not use are removed (D11).

## Buttons (2.2.3, decision D9)

| Place | Labels | Style | Behaviour |
|---|---|---|---|
| Button row, furthest right | Save and continue, Check eligibility, Continue to summary, Save and export, Add …, Generate report | Navy fill (#13294B), white Segoe UI Semibold 10, 40 high, flat, no border | The form's `AcceptButton` while its tab shows. Disabled (rule grey) while a blocking error stands. |
| Button row, to its left | Back, Update …, Preview, other secondary actions | White fill, blue (#1F5FA8) border, ink Segoe UI 10, 40 high | Back returns to the previous step and keeps captured data. |
| Button row, furthest left | Cancel | As above | The form's `CancelButton`. Abandons after a confirmation. |
| Inside a card, beside its group | Add subject, Find, Clear, Browse | Amber fill (#C8892B), ink Segoe UI Semibold 10, 30 high, at least 80 wide | Acts on its group and validates. Does not navigate. |

Each button has its own `btn<Tab><Action>_Click` handler in that tab's partial-class file.

## Fonts (2.2.4)

Segoe UI throughout. Consolas only for figures. Hierarchy comes from size and weight, never
colour. Semibold, never Bold.

| Use | Font |
|---|---|
| Screen title (staff login window) | Segoe UI Semibold 20 pt |
| Section (card) heading | Segoe UI Semibold 12 pt |
| Field label (above its control) | Segoe UI Semibold 12 pt |
| Body, inputs, grid cells, tabs, menu | Segoe UI 10 pt |
| Grid column headers | Segoe UI Semibold 10 pt, white on navy |
| Caption, helper text, disclaimers, sample-data notes, status bar | Segoe UI 8 pt, muted (#58606C) |
| Figures (marks, scores, dates, IDs) | Consolas 10 pt |

## Field labels (2.2.5)

Every input has a visible label directly above it, left-aligned, in sentence case with no
trailing colon. Units go in the label (e.g. "Mark % (0–100)"), never as placeholder text.
Required fields carry a red asterisk and the note "* indicates a required field." Set `TabIndex`
so the label comes before its control, and set `AccessibleName` on every input.

## Grids

`DataGridView` with row headers hidden, `AutoSizeColumnsMode = Fill`, white background, rule-grey
(#D7DCE3) grid lines, row height 30, column-header height 34, 16 px from the card edge.
Percentages use the format `0.#'%'`. Status columns show "✓ Active" (green) or
"Inactive" (muted) in Segoe UI Semibold 9.

## Colours (2.2.6) — one meaning each; colour is never the only signal

| Name | Hex | Meaning |
|---|---|---|
| Navy | `#13294B` | Structure: grid column headers, primary buttons, dialog headers |
| Deep navy | `#0B1F3A` | Status bar |
| Blue | `#1F5FA8` | Secondary button borders, guidance |
| Blue tint | `#E8EEF6` | Notice background for guidance, read-only fields, row banding |
| Amber | `#C8892B` | Outstanding or incomplete: amber notice and status, Add/Find buttons, **an unmet requirement** |
| Amber text | `#8A5D18` | Outstanding data |
| Red | `#A4262C` | **Invalid input only**, including a missing required field (D12), and the required-field asterisk |
| Green | `#1F6B3A` | Requirement met or complete. A notice turns green when the user can go on. |
| Ink / rule | `#1B1F24` / `#D7DCE3` | Text / dividers |

## Messages (2.2.7 and Section 7)

| Class | How it appears |
|---|---|
| Field message | Red "⚠" text under the control, which gets a red tint and a red label (`CheckField`). Raised when the control loses focus with an invalid value. |
| Validation summary | A red box above the controls, raised when the primary action is clicked while a field is invalid. UI 13 forms show one line, "⚠ n entries to correct: A, B."; UI 3 lists one line per entry. Focus moves to the first field to fix. |
| Validation dialog | A modal with a single button, raised when leaving a capture screen that still holds invalid data (UI 2.1a / 3.1a / 8.1a). |
| Confirmation request | Before leaving or submitting a session, exporting a report, deactivating reference data, signing out or closing. |
| Status message | The status bar: step or record position, count of outstanding errors, and success confirmations (**never a modal**). The signed-in user or the learner is on its right. |

Messages name the field and the correction. Learner-facing screens never show technical detail or
unhandled exceptions.

## Data entry

- The control matches the data type: ComboBox bound to its reference table (inactive rows
  excluded), NumericUpDown for numbers (both "." and "," accepted as the decimal point),
  DateTimePicker for dates. **No hard-coded `Items` collections anywhere.** This is a rubric line.
- Validate when a field loses focus, and again in the Business Logic Layer on submit.
- Deactivate reference data. Never delete it.

## Working in the Visual Studio Designer

- Open a form with **View Designer** on `MainForm.cs` / `StaffConsoleForm.cs` only. Open the other
  partial files with **View Code (F7)**: double-clicking them makes Visual Studio add a second
  `InitializeComponent` and break the build.
- Keep `InitializeComponent` plain: property assignments, `Controls.Add`, and `+=` event wiring.
