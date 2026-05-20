import React, { useEffect, useState } from 'react';
import { listOrders, getOrder } from './api.js';

export default function App() {
  const [orders, setOrders] = useState([]);
  const [selected, setSelected] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => { listOrders().then(setOrders); }, []);

  // BUG demo (frontend): no error UI for the 500 from Bob's broken order.
  async function open(id) {
    setError(null);
    try { setSelected(await getOrder(id)); }
    catch (e) { setError(String(e)); }
  }

  return (
    <div style={{ fontFamily: 'sans-serif', padding: 24 }}>
      <h1>Orders</h1>
      <ul>
        {orders.map(o => (
          <li key={o.id}>
            <button onClick={() => open(o.id)}>
              #{o.id} — {o.customerName}
            </button>
          </li>
        ))}
      </ul>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {selected && (
        <pre style={{ background: '#f4f4f4', padding: 12 }}>
          {JSON.stringify(selected, null, 2)}
        </pre>
      )}
    </div>
  );
}
