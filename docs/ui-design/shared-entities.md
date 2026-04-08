# ocent Shared Entities — UI Design Spec

This document specifies the UI patterns for cross-domain shared entities: Container, Tag, Link,
Source/Provenance, Status/Lifecycle, and User/Owner. These patterns appear across all four domains
(Finance, Documents, Storage, Contracts) and must behave identically regardless of context.

---

## Design Decisions

### Why These Are "Quiet" Components

Shared entity components are structural, not semantic. They exist to organize and annotate domain
data — not to be the focus. Every visual choice here prioritizes legibility of the domain data
they accompany over self-expression of the shared entity itself.

Rules derived from the ocent design philosophy:

- Provenance indicators never exceed `--text-muted` color — they must not compete with actual values
- Status badges show no color-only signaling — always icon + text label
- Tag chips are compact and flat — no gradient, no drop shadow
- Container cards use elevation to establish hierarchy, not color fills
- Links between domains are shown relationally, not as separate entity lists

---

## 1. Status Badge / Lifecycle Chip

**Selector:** `<ocent-status-chip>`

**Purpose:** Displays the processing lifecycle state of any entity (transaction, document, contract,
inventory item). States: `raw`, `processing`, `enriched`, `verified`, `archived`.

### Visual Description

```
┌──────────────────┐
│ ● Verified       │  height: 24px (default) / 20px (compact)
└──────────────────┘

Anatomy:
  [4px dot] [space-1] [text label]

  dot:   4px × 4px circle, border-radius: 50%
  text:  --text-xs, font-weight: 500
  bg:    semantic -subtle token
  text:  semantic -text token or matching foreground
  pad:   --space-2 horizontal, 0 vertical (height is fixed)
  radius: --radius-sm (4px)
```

### State → Token Mapping

| Lifecycle State | Dot Color Token          | Background Token           | Text Token               | Icon (Lucide)    |
|-----------------|--------------------------|----------------------------|--------------------------|------------------|
| `raw`           | `--text-muted`           | `--surface-overlay`        | `--text-secondary`       | `circle-dashed`  |
| `processing`    | `--color-info`           | `--color-info-subtle`      | `--color-info`           | `loader-circle`  |
| `enriched`      | `--color-warning`        | `--color-warning-subtle`   | `--color-warning`        | `sparkles`       |
| `verified`      | `--color-success`        | `--color-success-subtle`   | `--color-success`        | `circle-check`   |
| `archived`      | `--text-disabled`        | `--surface-sunken`         | `--text-disabled`        | `archive`        |

The `processing` state uses a rotating `loader-circle` icon (16px) instead of a static dot.
Respect `prefers-reduced-motion` — replace the rotation with a static icon when reduced motion is
active.

### Component API

```typescript
export type LifecycleStatus = 'raw' | 'processing' | 'enriched' | 'verified' | 'archived';

@Component({
  selector: 'ocent-status-chip',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StatusChipComponent {
  readonly status  = input.required<LifecycleStatus>();
  readonly compact = input(false);
  readonly showIcon = input(true);

  protected readonly config = computed(() => STATUS_CONFIG[this.status()]);
}
```

```html
<!-- Default usage -->
<ocent-status-chip status="verified" />

<!-- Compact variant (in dense tables) -->
<ocent-status-chip status="processing" [compact]="true" />

<!-- Icon-only with tooltip (most compact — table column) -->
<ocent-status-chip status="enriched" [showIcon]="true" [compact]="true"
  [title]="'Enriched — data augmented, awaiting verification'" />
```

### States

- **Default:** Background + dot + label as per table above
- **Hover (when inside clickable row):** No change — status chips are not themselves interactive
- **Loading (skeleton):** `80px × 24px` skeleton block, `--border-subtle` color, shimmer animation
- **Tooltip:** On hover/focus of the chip itself, show full lifecycle description via `ocent-tooltip`

### Accessibility

- The `<span>` wrapping the chip has no role — it is presentational with visible text
- The dot is `aria-hidden="true"` — the text label carries all meaning
- The `processing` rotating icon gets `aria-label="Processing"` on its wrapper and `aria-hidden` on
  the SVG itself
- Never use the chip in a context where only color distinguishes states — the text label is required
  (icon-only variant must have a `title` tooltip and `aria-label`)

### SCSS Notes

```scss
// status-chip.component.scss
.chip {
  display: inline-flex;
  align-items: center;
  gap: var(--space-1);
  padding: 0 var(--space-2);
  border-radius: var(--radius-sm);
  font-size: var(--text-xs);
  font-weight: 500;
  white-space: nowrap;
  font-family: var(--font-ui);

  &--default { height: 24px; }
  &--compact  { height: 20px; }

  &__dot {
    width: 4px;
    height: 4px;
    border-radius: 50%;
    flex-shrink: 0;
  }

  &__icon {
    width: 12px;
    height: 12px;
    flex-shrink: 0;

    &--spinning {
      animation: chip-spin 1.2s linear infinite;

      @media (prefers-reduced-motion: reduce) {
        animation: none;
      }
    }
  }
}

@keyframes chip-spin {
  from { transform: rotate(0deg); }
  to   { transform: rotate(360deg); }
}
```

---

## 2. Provenance Indicator

**Selector:** `<ocent-provenance-indicator>`

**Purpose:** Attached to any field value that was not manually typed by the user. Shows the origin
of the data — CSV import, OCR extraction, AI pipeline proposal, or manual override.

### Visual Description

```
Inline with a field value:

  EUR 1,284.00  [icon] imported
                └─── --text-xs, --text-muted

  EUR 1,284.00  [icon] OCR · 98%
                         └─── confidence, shown only for OCR

  EUR 1,284.00  [icon]
                  └─── icon-only in ultra-dense tables; tooltip on hover/focus

  Hover tooltip:
  ┌──────────────────────────────────────────┐
  │ Imported from CSV                        │
  │ Account export · 2026-01-10              │
  │ Source: Sparkasse export GIROKONTO       │
  └──────────────────────────────────────────┘
```

The indicator is always placed to the right of the value it annotates, never below or above.
In a `<ocent-data-def>` (detail drawer), it appears as a sub-line below the value.

### Source Types → Icon Mapping

| Source Type     | Icon (Lucide)      | Label         | Color         |
|-----------------|--------------------|---------------|---------------|
| `manual`        | `pencil-line`      | (hidden)      | —             |
| `manual-edit`   | `pencil`           | edited        | `--text-muted`|
| `import-csv`    | `file-input`       | imported      | `--text-muted`|
| `import-api`    | `plug`             | synced        | `--text-muted`|
| `ocr`           | `scan-text`        | OCR           | `--text-muted`|
| `ai-proposed`   | `cpu`              | AI proposed   | `--color-info`(muted — `hsl(199, 30%, 55%)`) |
| `pipeline`      | `workflow`         | pipeline      | `--text-muted`|

`manual` type renders nothing — the absence of the indicator means manually entered. `manual-edit`
appears when a user has overridden an auto-derived value.

The `ai-proposed` variant uses a slightly warmer-blue muted tint (`hsl(199, 30%, 55%)` dark mode /
`hsl(200, 40%, 42%)` light mode) to distinguish it visually from pure neutral muted text, without
being alarming. This is the only provenance type with a non-neutral color.

### Component API

```typescript
export type ProvenanceSource =
  | 'manual'
  | 'manual-edit'
  | 'import-csv'
  | 'import-api'
  | 'ocr'
  | 'ai-proposed'
  | 'pipeline';

export interface ProvenanceDetail {
  source: ProvenanceSource;
  label?: string;        // e.g. "Sparkasse export GIROKONTO"
  timestamp?: Date;
  confidence?: number;   // 0–1, shown as percentage for OCR only
  actor?: string;        // "finance-pipeline-v2", "user:mbarner"
}

@Component({
  selector: 'ocent-provenance-indicator',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProvenanceIndicatorComponent {
  readonly provenance = input.required<ProvenanceDetail>();
  readonly iconOnly   = input(false);
  readonly subline    = input(false); // detail drawer sub-line mode
}
```

```html
<!-- Inline with value (table cell or form) -->
<span class="cell__value">EUR 1,284.00</span>
<ocent-provenance-indicator
  [provenance]="{ source: 'import-csv', label: 'Sparkasse export', timestamp: importDate }"
  [iconOnly]="true" />

<!-- Detail drawer sub-line mode -->
<ocent-data-def label="Amount">
  <span slot="value">EUR 1,284.00</span>
  <ocent-provenance-indicator slot="meta"
    [provenance]="amountProvenance()"
    [subline]="true" />
</ocent-data-def>
```

### States

- **Manual source:** Renders nothing (`display: none` — not even the DOM element when source is `manual`)
- **Icon-only:** 14px icon, `--text-muted`, tooltip required via `ocent-tooltip`
- **Label variant:** 12px icon + `--text-xs --text-muted` label, no background
- **Sub-line:** Full inline line below value — icon, label, timestamp in `--text-xs --text-muted`
- **AI proposed — field not yet confirmed:** Border on the containing input shows `--color-info` at
  20% opacity as a subtle cue that this field awaits user review
- **Hover/focus on indicator:** Tooltip appears with full ProvenanceDetail formatted as:
  `"[Verb] by [actor/system] · [date]"` e.g. `"Extracted by OCR · 2026-01-14 · 98% confidence"`

### Tooltip Content Format

```
Source: import-csv
→ "Imported from CSV · Sparkasse GIROKONTO · 10 Jan 2026"

Source: ocr
→ "Extracted by OCR · 98% confidence · 14 Jan 2026"

Source: ai-proposed
→ "Proposed by finance pipeline · Not yet confirmed"

Source: manual-edit
→ "Manually edited · Was: [original value] · 15 Jan 2026"
```

### Accessibility

- The icon is `aria-hidden="true"` in all variants
- The label text (when visible) is sufficient for screen readers
- In icon-only mode, the wrapper has `role="img"` and `aria-label` containing the full tooltip text
- `manual` source renders no DOM — no empty elements left in the tree

### SCSS Notes

```scss
// provenance-indicator.component.scss
.provenance {
  display: inline-flex;
  align-items: center;
  gap: var(--space-1);
  color: var(--text-muted);
  font-size: var(--text-xs);
  font-family: var(--font-ui);
  cursor: default;

  &--ai-proposed {
    // Slightly warmer than muted but not alarming
    color: hsl(199, 30%, 55%);

    [data-theme='light'] & {
      color: hsl(200, 40%, 42%);
    }
  }

  &--subline {
    display: flex;
    align-items: center;
    gap: var(--space-1);
    margin-top: var(--space-1);
  }

  &__icon {
    width: 12px;
    height: 12px;
    flex-shrink: 0;
    color: inherit;
  }

  &__label {
    white-space: nowrap;
    color: inherit;
  }
}
```

---

## 3. Tag Chip

**Selector:** `<ocent-tag-chip>`

**Purpose:** Displays a single tag applied to any entity. Tags are free-form text with optional
domain scoping. Used both inline (with entity) and in filter bars.

### Visual Description

```
Default (no domain):
┌──────────────────┐
│  #tax-2025       │   height: 24px, --radius-full (pill)
└──────────────────┘

Domain-scoped:
┌──────────────────────────┐
│  [fin icon]  #bmw-loan   │   domain dot 4px left of label
└──────────────────────────┘

Removable (in tag-input context):
┌──────────────────────┐
│  #tax-2025  [×]      │   × is focusable, 14px, --text-muted → --text-primary on hover
└──────────────────────┘

Group of tags (entity inline display, max 3 visible):
  [#tax-2025] [#vehicle] [+2]      ← overflow chip shows count
```

### Visual Tokens

```scss
// Base tag (no domain)
background: var(--surface-overlay);
border: 1px solid var(--border-subtle);
color: var(--text-secondary);
border-radius: var(--radius-full);
height: 24px;
padding: 0 var(--space-2);
font-size: var(--text-xs);
font-weight: 500;

// Hover (when in clickable context — e.g. clicking to filter by tag)
background: var(--border-subtle);
border-color: var(--border-default);
color: var(--text-primary);
```

Domain-scoped tags use a 4px color dot (same tokens as status chip dot) before the label. The dot
color maps to the domain accent (`--color-finance`, `--color-documents`, etc.). The tag background
and text remain neutral — the dot is the only domain-colored element.

### Component API

```typescript
export type TagDomain = 'finance' | 'documents' | 'storage' | 'contracts' | null;

@Component({
  selector: 'ocent-tag-chip',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TagChipComponent {
  readonly tag        = input.required<string>();
  readonly domain     = input<TagDomain>(null);
  readonly removable  = input(false);
  readonly clickable  = input(false);
  readonly compact    = input(false); // 20px height

  readonly removed  = output<void>();
  readonly selected = output<string>();
}
```

```html
<!-- Static display (entity detail) -->
<ocent-tag-chip tag="tax-2025" />

<!-- Domain-scoped -->
<ocent-tag-chip tag="bmw-loan" domain="finance" />

<!-- Removable (in tag editor) -->
<ocent-tag-chip tag="tax-2025" [removable]="true" (removed)="removeTag('tax-2025')" />

<!-- Clickable (filter bar) -->
<ocent-tag-chip tag="urgent" [clickable]="true" (selected)="filterByTag($event)" />
```

### Overflow Group

When displaying tags inline on an entity card or table row, cap visible chips at 3. Remaining tags
collapse to an overflow chip: `[+N]` using the same neutral chip style. Clicking or hovering the
overflow chip reveals all tags in an `ocent-popover`.

```html
<!-- ocent-tag-group handles overflow internally -->
<ocent-tag-group [tags]="entity.tags()" [maxVisible]="3" />
```

### States

- **Default:** Neutral pill, border, secondary text
- **Hover (clickable):** Slightly brighter background, primary text, `cursor: pointer`
- **Focus:** 2px `--color-focus-ring` outline, 2px offset
- **Removing:** Chip fades out (`opacity 0`, `width 0`, `var(--duration-default)`) when removed
- **Loading:** `80px × 24px` skeleton pill

### Accessibility

- Non-clickable chips: `<span>` with no role, visible text only
- Clickable chips: `<button type="button">` with the tag text as accessible label
- Remove button: `<button type="button" aria-label="Remove tag tax-2025">` inside the chip
- Overflow chip `[+2]`: `<button aria-label="2 more tags" aria-haspopup="true">`
- `ocent-tag-group` announces tag count to screen readers via `aria-label` on the group container

### SCSS Notes

```scss
// tag-chip.component.scss
.tag {
  display: inline-flex;
  align-items: center;
  gap: var(--space-1);
  padding: 0 var(--space-2);
  border-radius: var(--radius-full);
  border: 1px solid var(--border-subtle);
  background: var(--surface-overlay);
  color: var(--text-secondary);
  font-size: var(--text-xs);
  font-weight: 500;
  white-space: nowrap;
  font-family: var(--font-ui);
  height: 24px;
  transition: background var(--duration-fast) var(--ease-default),
              border-color var(--duration-fast) var(--ease-default),
              color var(--duration-fast) var(--ease-default);

  &--compact { height: 20px; }

  &--clickable {
    cursor: pointer;

    &:hover, &:focus-visible {
      background: var(--border-subtle);
      border-color: var(--border-default);
      color: var(--text-primary);
    }

    &:focus-visible {
      outline: 2px solid var(--color-focus-ring);
      outline-offset: 2px;
    }
  }

  &__domain-dot {
    width: 4px;
    height: 4px;
    border-radius: 50%;
    flex-shrink: 0;
  }

  &__remove {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 14px;
    height: 14px;
    margin-left: var(--space-1);
    color: var(--text-muted);
    border: none;
    background: none;
    cursor: pointer;
    padding: 0;
    border-radius: var(--radius-sm);
    transition: color var(--duration-fast) var(--ease-default);

    &:hover, &:focus-visible {
      color: var(--text-primary);
    }

    &:focus-visible {
      outline: 2px solid var(--color-focus-ring);
      outline-offset: 1px;
    }
  }
}
```

---

## 4. Container Card

**Selector:** `<ocent-container-card>`

**Purpose:** Displays a Container — a named cross-domain grouping (e.g., "BMW X5", "Apartment
Berlin", "Freelance"). Shows the container name, a summary of linked domain entity counts, and
allows navigation into the container's detail view.

### Visual Description

```
┌──────────────────────────────────────────────────────┐
│  [layers icon]  BMW X5                    [›]        │
│                 Purchased 2022                        │
│                                                       │
│  [fin] 12 transactions   [doc] 8 documents            │
│  [con]  3 contracts      [sto] 2 items                │
│                                                       │
│  [#vehicle] [#bmw] [+1]              Verified        │
└──────────────────────────────────────────────────────┘

Width: 100% of parent column (not fixed-width)
Background: --surface-raised
Border: 1px solid --border-subtle
Border-radius: --radius-md (8px)
Padding: --space-5 (20px) — slightly denser than standard card
Elevation: --elevation-1

Domain summary row:
  Each domain count: [domain icon 14px] [count] [noun]
  Font: --text-sm, --text-secondary
  Domain icon color: the domain accent token (e.g. --color-finance)
  Displayed in a 2×2 grid at desktop, 2×2 at mobile

Footer row:
  Tags (left, via ocent-tag-group)
  Status chip (right, via ocent-status-chip)
```

### Collapsed / List Variant

For use in sidebar lists or within a domain page's "Related Containers" section:

```
[layers]  BMW X5       [fin:12] [doc:8] [con:3]  Verified  [›]
          Purchased 2022

One line, height: 56px, same surface and border tokens.
Domain counts are icon + number only (no noun) at compact density.
```

### Component API

```typescript
export interface ContainerSummary {
  id: string;
  name: string;
  description?: string;
  status: LifecycleStatus;
  tags: string[];
  domainCounts: {
    finance?: number;
    documents?: number;
    storage?: number;
    contracts?: number;
  };
}

@Component({
  selector: 'ocent-container-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContainerCardComponent {
  readonly container = input.required<ContainerSummary>();
  readonly variant   = input<'card' | 'list-item'>('card');

  readonly navigate = output<string>(); // emits container.id
}
```

```html
<ocent-container-card
  [container]="container()"
  (navigate)="onNavigate($event)" />

<ocent-container-card
  [container]="container()"
  variant="list-item"
  (navigate)="onNavigate($event)" />
```

### States

- **Default:** As described above
- **Hover:** `border-color` transitions to `--border-default`, subtle `background` lift to
  `--surface-overlay`. The `[›]` chevron shifts 2px right (`transform: translateX(2px)`,
  `--duration-fast`). `cursor: pointer`.
- **Focus:** 2px `--color-focus-ring` outline on the card's outer border, 2px offset
- **Loading:** Skeleton variant — title bar `180px × 18px`, description `100px × 14px`,
  domain count row `4× 80px × 14px` tiles, footer `3× tag skeleton`
- **Empty domains:** Domain count entries with 0 are omitted entirely (not shown as "0 documents")
- **Single domain:** If only one domain has data, show one count only — no empty grid cells

### Interaction

The entire card surface is a single focusable/clickable region that navigates to the container
detail page. Inside the card, the tag chips are individually interactive (filter on click) but use
`stopPropagation` to prevent triggering the card navigation.

### Accessibility

```html
<article
  role="article"
  [attr.aria-label]="'Container: ' + container().name"
  tabindex="0"
  (click)="navigate.emit(container().id)"
  (keydown.enter)="navigate.emit(container().id)"
  (keydown.space)="navigate.emit(container().id)">
  <!-- contents -->
</article>
```

- Heading inside card: `<h3>` with `--text-md` styling (not `font-size: var(--text-md)` on a div)
- Domain counts: Each count is a `<span>` with descriptive text — screen reader gets "12 finance
  transactions, 8 documents"
- The `[›]` chevron: `aria-hidden="true"` — navigation intent is conveyed by the article label

### SCSS Notes

File: `src/app/shared/ui/container-card/container-card.component.scss`

```scss
.container-card {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
  padding: var(--space-5);
  background: var(--surface-raised);
  border: 1px solid var(--border-subtle);
  border-radius: var(--radius-md);
  box-shadow: var(--elevation-1);
  cursor: pointer;
  transition:
    border-color var(--duration-fast) var(--ease-default),
    background var(--duration-fast) var(--ease-default);

  &:hover, &:focus-visible {
    border-color: var(--border-default);
    background: var(--surface-overlay);
  }

  &:focus-visible {
    outline: 2px solid var(--color-focus-ring);
    outline-offset: 2px;
  }

  &__header {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    gap: var(--space-3);
  }

  &__title-group {
    display: flex;
    align-items: center;
    gap: var(--space-2);
  }

  &__icon {
    color: var(--text-secondary);
    flex-shrink: 0;
  }

  &__title {
    font-size: var(--text-md);
    font-weight: 600;
    color: var(--text-primary);
    margin: 0;
  }

  &__description {
    font-size: var(--text-sm);
    color: var(--text-muted);
    margin: 0;
    padding-left: calc(20px + var(--space-2)); // align under title, past icon
  }

  &__chevron {
    color: var(--text-muted);
    flex-shrink: 0;
    transition: transform var(--duration-fast) var(--ease-default);
  }

  &:hover &__chevron,
  &:focus-visible &__chevron {
    transform: translateX(2px);
    color: var(--text-secondary);
  }

  &__domain-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: var(--space-2) var(--space-4);
  }

  &__domain-count {
    display: flex;
    align-items: center;
    gap: var(--space-1);
    font-size: var(--text-sm);
    color: var(--text-secondary);

    &-icon { flex-shrink: 0; }
  }

  &__footer {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: var(--space-2);
  }
}
```

---

## 5. Link / Relationship Display

**Selector:** `<ocent-entity-link-panel>`

**Purpose:** Shows cross-domain relationships on a Container detail page or within an entity's
detail drawer. A BMW X5 container might link to 12 finance transactions, 3 contracts, and 8
documents — this panel surfaces those relationships without requiring navigation.

### Visual Description

Container detail page — "Linked Records" section:

```
Linked Records
──────────────────────────────────────────────────────────

Finance                                             [View all 12 →]
┌───────────────────────────────────────────────────┐
│  [fin] Loan payment · Santander    –EUR 520.00    │
│  [fin] Insurance premium           –EUR 84.00     │
│  [fin] Service · BMW AG            –EUR 340.00    │
│  ··· 9 more transactions                          │
└───────────────────────────────────────────────────┘

Documents                                           [View all 8 →]
┌───────────────────────────────────────────────────┐
│  [doc] Vehicle registration · PDF  2025-03-14    │
│  [doc] Service report 2024 · PDF   2024-11-02    │
│  [doc] Purchase agreement · PDF    2022-08-15    │
│  ··· 5 more documents                            │
└───────────────────────────────────────────────────┘

Contracts                                           [View all 3 →]
  (same structure)
```

Domain sections that have zero linked records are hidden entirely (not shown as "No records").

Each domain section is visually separated by the domain accent color on a left border stripe
(`3px left border, domain accent at 60% opacity`).

```
Left accent border:
  border-left: 3px solid hsl(160, 84%, 39%, 0.6);  /* finance */
  border-left: 3px solid hsl(239, 84%, 67%, 0.6);  /* documents */
  border-left: 3px solid hsl(258, 90%, 66%, 0.6);  /* contracts */
  border-left: 3px solid hsl(38, 92%, 50%, 0.6);   /* storage */
```

### Single-Link Indicator (inline, compact)

Within a data table row or detail card, a compact badge signals that an entity belongs to a
container:

```
[layers icon 12px] BMW X5
```

Uses `--text-xs`, `--text-muted`, icon is `--color-interactive` at 70% opacity. Clicking navigates
to the container.

**Selector:** `<ocent-container-badge>`

```typescript
@Component({
  selector: 'ocent-container-badge',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContainerBadgeComponent {
  readonly containerId   = input.required<string>();
  readonly containerName = input.required<string>();

  readonly navigate = output<string>();
}
```

```html
<!-- In a transaction row -->
<ocent-container-badge
  containerId="ctr-bmw-x5"
  containerName="BMW X5"
  (navigate)="goToContainer($event)" />
```

### Link Panel Component API

```typescript
export interface LinkedDomainGroup {
  domain: TagDomain;
  totalCount: number;
  previewItems: LinkedEntityPreview[];
}

export interface LinkedEntityPreview {
  id: string;
  title: string;
  subtitle?: string;
  meta?: string;       // amount, date, file type
  status?: LifecycleStatus;
}

@Component({
  selector: 'ocent-entity-link-panel',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EntityLinkPanelComponent {
  readonly groups    = input.required<LinkedDomainGroup[]>();
  readonly loading   = input(false);
  readonly maxPreview = input(3);

  readonly viewAll  = output<TagDomain>();
  readonly navigate = output<{ domain: TagDomain; id: string }>();
}
```

### States

- **Loading:** Each domain section skeleton: title bar `120px`, then 3 rows `full-width × 36px`
  with shimmer
- **Empty (no links at all):** Show `ocent-empty-state` with `layers` icon, text
  `"No linked records. Add records to this container from any domain."`
- **Single domain only:** Render only the one domain section — no empty sections for others
- **Preview items hover:** Row background shifts to `--surface-overlay`, `cursor: pointer`

### Accessibility

- Each domain section is wrapped in `<section aria-labelledby="link-section-[domain]">`
- Section headings are `<h3>` — fits within container detail page `<h2>` sections
- "View all" links: `aria-label="View all 12 finance records for BMW X5"`
- Preview rows: `<a>` or `<button>` — keyboard navigable, focus ring visible
- Collapsed count "··· 9 more transactions": `role="note"`, not interactive (navigation is via "View all")

---

## 6. User / Owner Identity Display

**Selector:** `<ocent-user-badge>` (compact) / `<ocent-owner-info>` (detail)

**Purpose:** Displays the user identity in the topbar and ownership metadata on records. In a
self-hosted single-user system, this is mostly ambient confirmation ("you are mbarner").
Household membership contexts may show multiple owners.

### Topbar Badge (compact)

```
Avatar initials circle [20px] + display name (optional, hidden on narrow viewports)

┌───────────────────────────────────────────────┐
│  [M]  Marcel B.              ▾               │   topbar context
└───────────────────────────────────────────────┘

Avatar:
  - 28px circle, --radius-full
  - Background: --color-interactive-subtle (user-specific color TBD per household member)
  - Text: initials (1–2 chars), --text-xs, --font-ui, font-weight: 600
  - Color: --color-interactive
```

### Owner Metadata (detail drawer / record footer)

Shows who "owns" a record — relevant when household membership is active:

```
Owner: Marcel B.    Household: Barner Family
```

Displayed as `ocent-data-def` pairs using standard label/value styling. Not a custom component —
compose from existing `<ocent-data-def>` within context.

### Household Member Disambiguation

When multiple household members have access, an owner is shown as:
`[avatar 20px] [name, --text-sm]` inline. Selecting/filtering by owner uses the standard
`ocent-filter-bar` combobox pattern — no custom owner selector component.

### Component API

```typescript
export interface UserIdentity {
  id: string;
  displayName: string;
  initials: string;
  householdName?: string;
  householdRole?: 'owner' | 'member';
}

@Component({
  selector: 'ocent-user-badge',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserBadgeComponent {
  readonly user        = input.required<UserIdentity>();
  readonly showName    = input(true);
  readonly showChevron = input(false); // for menu trigger variant
}
```

### Accessibility

- Avatar circle: `aria-label="[displayName]"` on the button/link wrapper
- If the chevron indicates a dropdown: `aria-haspopup="menu"`, `aria-expanded` signal
- Avatar image (if photo added later): `alt="[displayName]"`
- Initials-only avatar: `aria-hidden="true"` on the text, `aria-label` on the wrapper

---

## 7. Source / Data Origin System (Composite)

The "source" concept in ocent spans two levels:

1. **Record-level source** — where did this record come from? (e.g., a transaction imported from a
   Sparkasse CSV, a contract uploaded manually, a document from OCR pipeline)
2. **Field-level provenance** — within a record, which specific field values were auto-derived?

### Record-Level Source Display

Displayed in the detail drawer using a standard `<ocent-data-def>` with the provenance indicator:

```
Origin
  [file-input icon]  CSV import · Sparkasse GIROKONTO · 10 Jan 2026
```

No special component — compose `ocent-provenance-indicator` with `ocent-data-def`.

### Import Source Chip (table column)

For tables with mixed-source records (e.g., a transaction list with both manual entries and imports),
a "Source" column may show compact chips:

```
[file-input]  CSV       ← import-csv
[plug]        API       ← import-api
[pencil-line] Manual    ← manual
[scan-text]   OCR       ← ocr
```

Same visual as `ocent-status-chip` in structure (height: 20px, `--radius-sm`) but using
provenance-specific icons and neutral coloring:

```scss
background: var(--surface-overlay);
border: 1px solid var(--border-subtle);
color: var(--text-secondary);
```

This reuses `ocent-status-chip` with a `variant="source"` input rather than introducing a new
component.

---

## 8. Component File Locations

```
src/app/shared/ui/
  status-chip/
    status-chip.component.ts
    status-chip.component.html
    status-chip.component.scss
  provenance-indicator/
    provenance-indicator.component.ts
    provenance-indicator.component.html
    provenance-indicator.component.scss
  tag-chip/
    tag-chip.component.ts
    tag-chip.component.html
    tag-chip.component.scss
  tag-group/
    tag-group.component.ts
    tag-group.component.html
    tag-group.component.scss
  container-card/
    container-card.component.ts
    container-card.component.html
    container-card.component.scss
  container-badge/
    container-badge.component.ts
    container-badge.component.html
    container-badge.component.scss
  entity-link-panel/
    entity-link-panel.component.ts
    entity-link-panel.component.html
    entity-link-panel.component.scss
  user-badge/
    user-badge.component.ts
    user-badge.component.html
    user-badge.component.scss
```

All components are in `src/app/shared/ui/` and exported from a barrel:
`src/app/shared/ui/index.ts`.

---

## 9. Design Tokens in Use — Quick Reference

| Purpose                          | Token                          |
|----------------------------------|--------------------------------|
| Card background                  | `--surface-raised`             |
| Card border (default)            | `--border-subtle`              |
| Card border (hover)              | `--border-default`             |
| Card elevation                   | `--elevation-1`                |
| Card border-radius               | `--radius-md`                  |
| Tag/chip pill radius             | `--radius-full`                |
| Status chip radius               | `--radius-sm`                  |
| Provenance / muted text          | `--text-muted`                 |
| Secondary labels                 | `--text-secondary`             |
| Primary data values              | `--text-primary`               |
| Caption / meta / timestamps      | `--text-xs` size token         |
| Domain icon tint (finance)       | `--color-finance`              |
| Domain icon tint (documents)     | `--color-documents`            |
| Domain icon tint (storage)       | `--color-storage`              |
| Domain icon tint (contracts)     | `--color-contracts`            |
| Tag background                   | `--surface-overlay`            |
| Hover fill (interactive)         | `--border-subtle`              |
| Focus ring                       | `--color-focus-ring`           |
| Transition duration (hover)      | `--duration-fast` (120ms)      |
| Transition easing                | `--ease-default`               |
| Processing spin animation        | 1.2s linear                    |
| Avatar background                | `--color-interactive-subtle`   |
| Avatar text                      | `--color-interactive`          |

---

## 10. Shared Entity Interaction Summary

| Entity             | Interactive? | Navigation target                        | Keyboard support              |
|--------------------|-------------|------------------------------------------|-------------------------------|
| `status-chip`      | No (display)| —                                        | Not focusable unless in row   |
| `provenance-indicator` | Hover tooltip | —                               | Tooltip on focus (icon-only)  |
| `tag-chip`         | Optional    | Filter by tag (same page)                | Enter/Space to activate       |
| `container-card`   | Yes         | Container detail page                    | Enter/Space on card           |
| `container-badge`  | Yes         | Container detail page                    | Enter/Space                   |
| `entity-link-panel`| Yes (rows)  | Domain entity detail drawer              | Tab through rows, Enter       |
| `user-badge`       | Yes         | User settings or owner menu              | Enter/Space                   |

---

## 11. Cross-Domain Color Discipline

These shared components must never introduce domain-accent color unless explicitly scoping to a
domain. Rules:

- `ocent-status-chip` uses only semantic tokens (success, warning, error, info, neutral) — never
  domain accent tokens
- `ocent-provenance-indicator` uses only `--text-muted` / `--text-secondary` — never domain accent
- `ocent-tag-chip` uses neutral tokens; the optional 4px domain dot uses domain accent — the chip
  body remains neutral
- `ocent-container-card` uses neutral surfaces; domain counts show domain icon in accent color,
  but card frame is fully neutral
- `ocent-entity-link-panel` uses the left-border accent stripe as the only domain color signal —
  section content text remains `--text-secondary`

This discipline ensures that when domain colors appear, they carry information — not just style.
