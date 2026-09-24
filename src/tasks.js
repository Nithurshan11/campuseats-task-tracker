// CampusEats task list
const tasks = [
  { title: "Design the menu screen", dueDate: "2026-10-01" },
  { title: "Build the orders API", dueDate: "2026-10-08" },
  { title: "Add user login", dueDate: "2026-10-15" },
];

console.log(`CampusEats has ${tasks.length} open tasks`);

// Clear names, no magic numbers, no secrets.
const VIP_DISCOUNT = 0.1;

function calculateTotal(price, quantity, customerType) {
  if (price < 0 || quantity < 0) {
    throw new Error("price and quantity must be >= 0");
  }

  const subtotal = price * quantity;
  return customerType === "vip" ? subtotal * (1 - VIP_DISCOUNT) : subtotal;
}

module.exports = { tasks, calculateTotal, VIP_DISCOUNT };
