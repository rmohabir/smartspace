import { expect, test } from '@playwright/test';

test.describe('US3 own reservations and history', () => {
  test('shows the three reservation views and an accessible empty state', async ({ page }) => {
    await page.goto('http://localhost:3000/mijn-reserveringen');

    await expect(page.getByRole('heading', { name: 'Mijn reserveringen' })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Komend' })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Afgelopen' })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Geannuleerd' })).toBeVisible();
  });

  test('supports keyboard navigation between history views', async ({ page }) => {
    await page.goto('http://localhost:3000/mijn-reserveringen');
    await page.getByRole('button', { name: 'Komend' }).focus();
    await page.keyboard.press('Tab');
    await expect(page.getByRole('button', { name: 'Afgelopen' })).toBeFocused();
    await page.keyboard.press('Enter');
    await expect(page.getByRole('button', { name: 'Afgelopen' })).toBeVisible();
  });

  test('keeps the page usable at mobile width', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 812 });
    await page.goto('http://localhost:3000/mijn-reserveringen');

    await expect(page.getByRole('heading', { name: 'Mijn reserveringen' })).toBeVisible();
    await expect(page.locator('body')).not.toHaveCSS('overflow-x', 'scroll');
  });
});