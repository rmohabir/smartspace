import { expect, test } from '@playwright/test';

test.describe('US1 room availability', () => {
  test('shows the availability search form and accessible filters', async ({ page }) => {
    await page.goto('http://localhost:3000/beschikbaarheid');

    await expect(page.getByRole('heading', { name: 'Beschikbaarheid' })).toBeVisible();
    await expect(page.getByLabel('Begintijd')).toBeVisible();
    await expect(page.getByLabel('Eindtijd')).toBeVisible();
    await expect(page.getByLabel('Locatie')).toBeVisible();
    await expect(page.getByLabel('Minimumcapaciteit')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Zoek beschikbare ruimtes' })).toBeVisible();
  });

  test('validates an interval before sending a request', async ({ page }) => {
    await page.goto('http://localhost:3000/beschikbaarheid');
    await page.getByLabel('Begintijd').fill('2026-09-16T11:00');
    await page.getByLabel('Eindtijd').fill('2026-09-16T10:00');
    await page.getByRole('button', { name: 'Zoek beschikbare ruimtes' }).click();

    await expect(page.getByRole('alert')).toContainText('eindtijd moet na de begintijd');
  });

  test.describe('responsive layouts', () => {
    for (const width of [375, 768, 1440]) {
      test(`renders without horizontal overflow at ${width}px`, async ({ page }) => {
        await page.setViewportSize({ width, height: 900 });
        await page.goto('http://localhost:3000/beschikbaarheid');
        await expect(page.locator('body')).toHaveCSS('overflow-x', 'visible');
      });
    }
  });
});
