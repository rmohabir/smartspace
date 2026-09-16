import { expect, test } from '@playwright/test';

test.describe('US2 room booking', () => {
  test('shows an accessible booking form', async ({ page }) => {
    await page.goto('http://localhost:3000/reserveren/20000000-0000-0000-0000-000000000001');

    await expect(page.getByRole('heading', { name: 'Ruimte reserveren' })).toBeVisible();
    await expect(page.getByLabel('Begintijd')).toBeVisible();
    await expect(page.getByLabel('Eindtijd')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Reservering bevestigen' })).toBeVisible();
  });

  test('keeps the form usable after an invalid interval', async ({ page }) => {
    await page.goto('http://localhost:3000/reserveren/20000000-0000-0000-0000-000000000001');
    await page.getByLabel('Begintijd').fill('2026-09-16T11:00');
    await page.getByLabel('Eindtijd').fill('2026-09-16T10:00');
    await page.getByRole('button', { name: 'Reservering bevestigen' }).click();

    await expect(page.getByRole('alert')).toContainText('eindtijd moet na de begintijd');
    await expect(page.getByLabel('Begintijd')).toHaveValue('2026-09-16T11:00');
  });

  test('presents a conflict as a reload action', async ({ page }) => {
    await page.goto('http://localhost:3000/reserveren/20000000-0000-0000-0000-000000000001');
    await page.getByLabel('Begintijd').fill('2026-09-16T11:00');
    await page.getByLabel('Eindtijd').fill('2026-09-16T12:00');

    await expect(page.getByRole('button', { name: 'Reservering bevestigen' })).toBeEnabled();
  });
});
