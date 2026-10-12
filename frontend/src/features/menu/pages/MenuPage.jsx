import { useState } from "react";
import MenuList from "../components/MenuList";
import { useDebounce } from "../hooks/useDebounce";
import { useFetch } from "../hooks/useFetch";


function MenuPage() {
  // Read API base URL from environment (Task 01)
  const API = import.meta.env.VITE_API_URL;
  const menuEndpoint = API ? `${API}/api/menu` : "/menu.json";

  const { 
    data: dishes, 
    isLoading, 
    error 
  } = useFetch(menuEndpoint);


  const [query, setQuery] = useState("");

  const debounced = useDebounce(query, 400);



  // Loading state
  if (isLoading) {
    return <p>Loading menu...</p>;
  }

  // Error state
  if (error) {
    return (
      <p>
        Could not load menu: {error}
      </p>
    );
  }

  // Success state

  const filtered = dishes.filter((dish) =>
    dish.name
      .toLowerCase()
      .includes(debounced.toLowerCase())
  );


  return (

    <div>

      <h1>CampusEats Menu</h1>
      <input
        value={query}
        onChange={(e)=>setQuery(e.target.value)}
        placeholder="Search dishes..."
      />

    <MenuList dishes={filtered}/>


    </div>

  );
}


export default MenuPage;