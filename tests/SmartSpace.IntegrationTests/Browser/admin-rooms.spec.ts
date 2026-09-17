import { expect, test } from '@playwright/test';

test.describe('US4 room administration', () => {
  test('shows the admin room management form', async ({ page }) => {
    await page.goto('http://localhost:3000/beheer/ruimtes');

    await expect(page.getByRole('heading', { name: 'Ruimtes beheren' })).toBeVisible();
    await expect(page.getByLabel('Naam')).toBeVisible();
    await expect(page.getByLabel('Capaciteit')).toBeVisible();
    await expect(page.getByLabel('Locatie')).toBeVisible();
    await expect(page.getByLabel('Locatie').locator('option').first()).toHaveText('Kies een locatie');
    await expect(page.getByRole('button', { name: 'Opslaan' })).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Locatie toevoegen' })).toBeVisible();
  });

  test('shows inline validation for incomplete room data', async ({ page }) => {
    await page.goto('http://localhost:3000/beheer/ruimtes');
    await page.getByRole('button', { name: 'Opslaan' }).click();

    await expect(page.getByRole('alert')).toContainText('naam');
  });

  test('supports keyboard operation on the room form at mobile width', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 812 });
    await page.goto('http://localhost:3000/beheer/ruimtes');
    await page.getByLabel('Naam').focus();
    await page.keyboard.type('Nieuwe ruimte');
    await page.keyboard.press('Tab');
    await expect(page.getByLabel('Capaciteit')).toBeFocused();
  });

  test('allows a location to be edited without typing an identifier', async ({ page }) => {
    await page.goto('http://localhost:3000/beheer/ruimtes');
    await expect(page.getByRole('heading', { name: 'Locaties' })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Wijzigen' }).last()).toBeVisible();
  });
});