const BASE = 'http://localhost:5080/api';

export async function listOrders() {
  const r = await fetch(`${BASE}/orders`);
  return r.json();
}

export async function getOrder(id) {
  const r = await fetch(`${BASE}/orders/${id}`);
  if (!r.ok) throw new Error(`HTTP ${r.status}`);
  return r.json();
}

export async function listProducts() {
  const r = await fetch(`${BASE}/products`);
  return r.json();
}
