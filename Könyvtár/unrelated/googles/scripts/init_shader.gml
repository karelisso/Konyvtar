//init_shader.gml


set_character_color_shading(7, 2.2);
switch (get_player_color(player)) {
	case 0:
		set_character_color_slot( 5, 66, 66, 66 ); //Shorts
		set_character_color_slot( 7, 66, 66, 66 ); //Shoes2
		break;
	case 1:
		break;
	case 2:
		break;
	case 3:
		break;
	case 4:
		break;	
	case 6:
		break;
	case 7:
		set_character_color_shading(1, -1.1);	
		break;
	case 8:
		set_character_color_shading(1, -1.5);	
		break;
	case 9:
		set_character_color_shading(1, 2);	
		break;
	case 10:
		set_character_color_shading(1, -1.1);	
		break;	
	case 12:
		set_character_color_shading(1, 0.8);	
		break;
	case 13:
		set_character_color_shading(1, .5);		
		break;	
	case 14:
		set_character_color_shading(1, 1);	
		break;	
	case 15:
		set_character_color_shading(1, 2);	
		break;	
	case 16:	
		break;	
	case 17:	
		break;		
	case 18:
		break;	
	case 19:	
		break;		
	case 20:
		set_character_color_shading(1, -1.1);
		break;		
	case 21:
		set_character_color_shading(1, 1.5);	
		set_character_color_shading(2, 1.2);		
		break;		
	case 22:
		set_character_color_shading(1, 1.5);	
		set_character_color_shading(2, 0.7);		
		break;			
	case 23:
		set_character_color_shading(1, 1.2);	
		set_character_color_shading(2, 1.2);		
		set_character_color_shading(4, -0.6);		
		set_character_color_shading(5, -0.6);		
		break;			
	case 24:
		set_character_color_shading(1, 1.2);	
		set_character_color_shading(2, 0.5);	
		set_character_color_shading(4, 0.5);		
		set_character_color_shading(5, 0.5);	
		//set_character_color_shading(4, sin((.23 * hue) + 1.5));		
		//set_character_color_shading(5, sin((.23 * hue) + 1.5));
		break;			
	case 25:	
		set_character_color_shading(0, 0);
		set_character_color_shading(1, 0);
		set_character_color_shading(2, 0);
		set_character_color_shading(3, 0);
		set_character_color_shading(4, 0);	
		set_character_color_shading(5, 0);	
		set_character_color_shading(6, 0);	
		set_character_color_shading(7, 0);	
		break;			
	case 26:	
	set_character_color_shading(1, -1.5);	
		break;			
	case 27:			
		set_character_color_shading(1, 0.6);	
		break;			
	case 28:			
		set_character_color_shading(1, -1);
		set_character_color_shading(2, -1);
		break;			
	case 29:			
		set_character_color_shading(1, -0.6);	
		break;		
}




