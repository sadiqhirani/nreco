<script runat="server" language="c#">
</script>


<html>
	<head>
	<link href='css/styles.css' rel='stylesheet' rev='stylesheet' type='text/css' />
	<script language="javascript" src="js/common.js"></script>
	<script language="javascript" src="js/window.js"></script>	
	</head>
<body>

<form runat="server">
<%=WebManager.GetService<IProvider>("aaa").Provide(null) %>
	
</form>

</body>
</html>