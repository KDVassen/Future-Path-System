# Future Path — screen standards (spec Section 2.2)

Applies to every screen, using standard .NET Framework Windows Forms controls with no custom
painting libraries. The spec wording wins. The hex values are the approved "Academic Navy Rail"
palette the spec's colour names refer to.

## Form layout (2.2.1)
- `FormBorderStyle = FixedSingle`, `MaximizeBox = false`, `StartPosition = CenterScreen`,
  `AutoScaleMode = Dpi`.
- Sizes: **wide 1200 × 800** (capture and record screens), **standard 1000 × 720** (outcome,
  hub and selection screens), and **dialog 520 × 300** for modals. All fit 1400 × 800 without
  scrolling.
- **Every screen has these zones:** a caption bar reading "Future Path — <screen name>", a
  navigation rail docked left, a title band with the screen name, an action band docked bottom,
  and a status band.
- **Conditional zones:** a learner context strip, an amber notice panel, and up to four
  running-figure tiles. Each is **hidden, never shown empty**.
- About **20 px** between groups of controls. Use a TableLayoutPanel wherever things must stay
  aligned.

## Navigation (2.2.2)
- The rail is the main way to move between areas. **Cancel, Back and Next/primary** sit in the
  action band.
- The learner screens UI 2–7 run in a fixed order, and the rail shows progress. Staff screens
  can be reached in any order.

## Buttons (2.2.3)

| Position | Labels | Style | Behaviour |
|---|---|---|---|
| Furthest right | Save, Continue, Next, Generate, Print, Check eligibility | Navy fill, white text | The form's `AcceptButton`. Disabled while a blocking validation error stands. |
| Second from right | Back | White fill, blue border, black text | Returns to the previous screen and keeps captured data. |
| Third from right | Cancel | White fill, blue border, black text | The form's `CancelButton`. Abandons after a confirmation. |
| Beside its group | Add subject, Search, Clear | Amber fill, black text | Acts on its group and validates. Does not navigate. |

## Fonts (2.2.4)
Segoe UI throughout. Consolas only for figures. Hierarchy comes from size and weight, never
colour. Semibold, never Bold.

| Use | Font |
|---|---|
| Screen title | Segoe UI Semibold 20 pt |
| Section heading | Segoe UI Semibold 12 pt |
| Body, grid cells | Segoe UI 10 pt |
| Field label (above its control) | Segoe UI Semibold 12 pt |
| Caption, helper text, status band | Segoe UI 8 pt |
| Figures (marks, scores, dates, IDs) | Consolas 10 pt |

## Field labels (2.2.5)
Every input has a visible label directly above it, left-aligned, in sentence case with no
trailing colon. Units go in the label (e.g. "Mark % (0–100)"), never as placeholder text.
Set `TabIndex` so the label comes before its control, and set `AccessibleName` on every input.

## Colours (2.2.6) — one meaning each; colour is never the only signal

| Name | Hex | Meaning |
|---|---|---|
| Navy | `#13294B` | Structure: rail, title band, grid column headers, primary buttons |
| Deep navy | `#0B1F3A` | Frame outline and status bar |
| Blue | `#1F5FA8` | Context strip and secondary button borders, reference/guidance |
| Blue tint | `#E8EEF6` | Banding, context strip fill, grid rows |
| Amber | `#C8892B` | Outstanding or incomplete: notice panel, progress, Add buttons, **an unmet requirement** |
| Amber text | `#8A5D18` | Outstanding data |
| Red | `#A4262C` | **Invalid input only** (and the required-field asterisk) |
| Green | `#1F6B3A` | Requirement met or complete. The notice panel turns green when the user can go on. |
| Ink / rule | `#1B1F24` / `#D7DCE3` | Text / dividers |

## Messages (2.2.7 and Section 7)

| Class | How it appears |
|---|---|
| Field message | Red text with a warning glyph under the control, which gets a red border and tint. Raised when the control loses focus with an invalid value. |
| Validation summary | A red-bordered box above the controls, one line per bad field. Raised when the primary action is clicked while any field is invalid. |
| Validation dialog | A modal with a single button, raised when leaving a capture screen that still holds invalid data (UI 2.1a / 3.1a / 8.1a). |
| Confirmation request | Before submitting a session, exporting a report, or deactivating a degree or funding option. |
| Status message | The status band: signed-in user and role, step or record position, count of outstanding errors, and success confirmations (**never a modal**). |

Messages name the field and the correction. Learner-facing screens never show technical
detail or unhandled exceptions.

## Data entry
- The control matches the data type: ComboBox bound to its reference table (inactive rows
  excluded), NumericUpDown for numbers, DateTimePicker for dates. **No hard-coded `Items`
  collections anywhere.** This is a rubric line.
- Validate when a field loses focus, and again in the Business Logic Layer on submit.
- Deactivate reference data. Never delete it.
